using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using JetBrains.Annotations;
using NSrtm.Core;
using ReactiveUI;
using Splat;

namespace NSrtm.Demo
{
    internal sealed class DemoViewModel : ReactiveObject
    {
        private const int pixelWidthInt = 1600;
        private const int pixelHeightInt = 900;
        private const int pixelCount = pixelWidthInt * pixelHeightInt;
        private readonly WriteableBitmap _writeableBitmap;
        private readonly ReactiveCommand<ElevationValueStats> _retrieveElevationsCommand;
        private IEnumerable<IElevationProvider> _availableElevationProviders;
        [CanBeNull] private IElevationProvider _selectedElevationProvider;
        private double _centerLatitude = 50;
        private double _centerLongitude = 20;
        private double _rangeDegree = 1;
        private ElevationValueStats _elevationValueStats;
        private float _minVisualizedHeight = -100;
        private float _maxVisualizedHeight = 2500;
        private readonly ObservableAsPropertyHelper<double> _progress;
        public double Progress { get { return _progress.Value; } }
        private Subject<double> progresSubject = new Subject<double>();
        

        public DemoViewModel(IEnumerable<IElevationProvider> providers = null)
        {
            _availableElevationProviders = providers ?? Locator.Current.GetService<IEnumerable<IElevationProvider>>();
            if (_availableElevationProviders == null) throw new ArgumentNullException("providers");

            _writeableBitmap = new WriteableBitmap(pixelWidthInt, pixelHeightInt, 96, 96, PixelFormats.Rgb24, null);
            var epAvailable = this.WhenAnyValue(t => t.SelectedElevationProvider)
                                  .Select(sep => sep != null);
            _retrieveElevationsCommand = ReactiveCommand.CreateAsyncTask(epAvailable, _ => refreshElevationsAsync());

            _retrieveElevationsCommand.ObserveOn(RxApp.MainThreadScheduler)
                                      .Subscribe(s => ElevationValueStats = s);
            _retrieveElevationsCommand.ThrownExceptions.Subscribe(e => UserError.Throw("Error during retrieval of elevation data", e));
            _selectedElevationProvider = _availableElevationProviders.FirstOrDefault();

            _progress = progresSubject.ToProperty(this, t => t.Progress, scheduler: RxApp.MainThreadScheduler);
        }

        public float MinVisualizedHeight
        {
            get { return _minVisualizedHeight; }
            set
            {
                this.RaisePropertyChanging();
                _minVisualizedHeight = Math.Min(_maxVisualizedHeight - 1, value);
                this.RaisePropertyChanged();
            }
        }

        public float MaxVisualizedHeight
        {
            get { return _maxVisualizedHeight; }
            set
            {
                this.RaisePropertyChanging();
                _maxVisualizedHeight = Math.Max(_minVisualizedHeight + 1, value);
                this.RaisePropertyChanged();
            }
        }

        [NotNull] public IEnumerable<IElevationProvider> AvailableElevationProviders
        {
            get { return _availableElevationProviders; }
            set { this.RaiseAndSetIfChanged(ref _availableElevationProviders, value); }
        }

        [CanBeNull] public IElevationProvider SelectedElevationProvider
        {
            get { return _selectedElevationProvider; }
            set { this.RaiseAndSetIfChanged(ref _selectedElevationProvider, value); }
        }

        public double CenterLatitude { get { return _centerLatitude; } set { this.RaiseAndSetIfChanged(ref _centerLatitude, value); } }
        public double CenterLongitude { get { return _centerLongitude; } set { this.RaiseAndSetIfChanged(ref _centerLongitude, value); } }
        public double RangeDegree { get { return _rangeDegree; } set { this.RaiseAndSetIfChanged(ref _rangeDegree, value); } }
        [NotNull] public WriteableBitmap ElevationBitmap { get { return _writeableBitmap; } }

        public ElevationValueStats ElevationValueStats
        {
            get { return _elevationValueStats; }
            set { this.RaiseAndSetIfChanged(ref _elevationValueStats, value); }
        }

        public ReactiveCommand<ElevationValueStats> RetrieveElevationsCommand { get { return _retrieveElevationsCommand; } }
        // ReSharper disable once StringLiteralTypo
        [DllImport("shlwapi.dll")]
        public static extern int ColorHLSToRGB(int H, int L, int S);

        [NotNull]
        private async Task<ElevationValueStats> refreshElevationsAsync()
        {
            var ep = SelectedElevationProvider;
            if (ep == null)
            {
                MessageBox.Show("No ElevationProvider selected");
                return new ElevationValueStats(float.NaN, float.NaN, float.NaN, float.NaN, TimeSpan.Zero, pixelCount, 0);
            }

            var s = Stopwatch.StartNew();
            var elevations = await retrieveElevationAsync(ep, CenterLatitude, CenterLongitude, RangeDegree);
            s.Stop();
            var time = s.Elapsed;

            var statsTask = elevationStatsAsync(elevations.SelectMany(row => row), time);

            writeToBitmap(elevations, MinVisualizedHeight, MaxVisualizedHeight);

            return await statsTask;
        }

        private void writeToBitmap([NotNull] float[][] elevations, float minVisualizedHeight, float maxVisualizedHeight)
        {
            _writeableBitmap.Lock();

            unsafe
            {
                // Get a pointer to the back buffer. 
                byte* pBackBuffer = (byte*)_writeableBitmap.BackBuffer;

                for (int rowIdx = 0; rowIdx < pixelHeightInt; rowIdx++)
                {
                    byte* row = pBackBuffer + rowIdx * _writeableBitmap.BackBufferStride / sizeof(byte);
                    for (int i = 0; i < pixelWidthInt; i++)
                    {
                        float height = elevations[rowIdx][i];
                        uint colorRgb;
                        if (!float.IsNaN(height))
                        {
                            float relativeHeight = (height - minVisualizedHeight) / (maxVisualizedHeight - minVisualizedHeight);
                            const int maxHue = 240;
                            int hue = (int)Math.Max(0, Math.Min(relativeHeight * maxHue, maxHue));
                            colorRgb = (uint)ColorHLSToRGB(hue, 100, 240);
                        }
                        else
                        {
                            colorRgb = 0;
                        }

                        *row = (byte)(colorRgb >> 16);
                        row++;
                        *row = (byte)(colorRgb >> 8);
                        row++;
                        *row = (byte)(colorRgb >> 0);
                        row++;
                    }
                }
            }

            // Specify the area of the bitmap that changed.
            _writeableBitmap.AddDirtyRect(new Int32Rect(0, 0, pixelWidthInt, _writeableBitmap.PixelHeight));

            // Release the back buffer and make it available for display.
            _writeableBitmap.Unlock();
        }

        private static Task<ElevationValueStats> elevationStatsAsync(IEnumerable<float> allElevations, TimeSpan time)
        {
            return Task.Run(() => elevationStatsAsyncImpl(allElevations, time));
        }

        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        private static ElevationValueStats elevationStatsAsyncImpl(IEnumerable<float> allElevationsLinear, TimeSpan time)
        {
            var allElevations = allElevationsLinear.Where(v => !float.IsNaN(v));

            int count = 0;

            float elevationMax = float.NegativeInfinity;
            float elevationMin = float.PositiveInfinity;
            double sum = 0;

            foreach (var sample in allElevations)
            {
                elevationMax = Math.Max(elevationMax, sample);
                elevationMin = Math.Min(elevationMin, sample);
                sum += sample;
                count++;
            }

            float elevationDiff = elevationMax - elevationMin;
            float elevationAvg = (float)(count > 0 ? sum / count : 0);
            return new ElevationValueStats(elevationMin, elevationMax, elevationDiff, elevationAvg, time, pixelCount, count);
        }

        [NotNull]
        private Task<float[][]> retrieveElevationAsync(
            [NotNull] IElevationProvider ep,
            double latCenter,
            double lonCenter,
            double range)
        {
            if (ep == null) throw new ArgumentNullException("ep");

            return Task.Run(() => retrieveElevationAsyncImpl(ep, latCenter, lonCenter, range));
        }

        [NotNull]
        private float[][] retrieveElevationAsyncImpl(
            [NotNull] IElevationProvider ep,
            double latCenter,
            double lonCenter,
            double range)
        {
            if (ep == null) throw new ArgumentNullException("ep");

            double minLat = Math.Max(latCenter - range, -90);
            double maxLat = Math.Min(latCenter + range, 90);

            double minLon = Math.Max(lonCenter - range, -180);
            double maxLon = Math.Min(lonCenter + range, 180);

            double latRange = maxLat - minLat;
            double lonRange = maxLon - minLon;

            float[][] result = new float[pixelHeightInt][];

            for (int rIdx = 0; rIdx < pixelHeightInt; rIdx++)
            {
                double lat = minLat + rIdx * latRange / pixelHeightInt;
                result[rIdx] = retrieveRowElevation(ep, lat, minLon, lonRange);
                progresSubject.OnNext(rIdx*1.0 / pixelHeightInt);
            }

            return result;
        }

        [NotNull]
        private static float[] retrieveRowElevation(IElevationProvider ep, double lat, double minLon, double lonRange)
        {
            if (ep == null) throw new ArgumentNullException("ep");

            var rowArray = new float[pixelWidthInt];

            for (int cIdx = 0; cIdx < pixelWidthInt; cIdx++)
            {
                double lon = minLon + cIdx * lonRange / pixelWidthInt;
                var elevation = (float)ep.GetElevation(lat, lon);
                rowArray[cIdx] = elevation;
            }
            return rowArray;
        }
    }
}
