using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using NSrtm.Core;

namespace NSrtm.Demo
{
    public partial class MainWindow
    {
        private readonly WriteableBitmap _writeableBitmap;

        public MainWindow()
        {
            InitializeComponent();
            ElevationImage.MouseLeftButtonUp += ElevationImage_MouseLeftButtonUp;
            _writeableBitmap = new WriteableBitmap(2000, 1000, 96, 96, PixelFormats.Gray32Float, null);
            ElevationImage.Source = _writeableBitmap;
        }

        public IEnumerable<IElevationProvider> ElevationProviders
        {
            get { return ElevationModeCombo.ItemsSource as IEnumerable<IElevationProvider>; }
            set
            {
                ElevationModeCombo.ItemsSource = value;
                if (value != null) ElevationModeCombo.SelectedItem = value.FirstOrDefault();
            }
        }

        private async void ElevationImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            double latCenter = LatitudeSlider.Value;
            double lonCenter = LongitudeSlider.Value;
            double range = AreaSlider.Value;

            var ep = ElevationModeCombo.SelectedItem as IElevationProvider;
            if (ep == null)
            {
                return;
            }

            int pixelWidthInt = _writeableBitmap.PixelWidth;
            int pixelHeightInt = _writeableBitmap.PixelHeight;

            var elevations = await retrieveElevationAsync(ep, pixelHeightInt, pixelWidthInt, latCenter, lonCenter, range);

            var allElevations = elevations.SelectMany(row => row);

            var stats = await elevationStatsAsync(allElevations);

            _writeableBitmap.Lock();

            unsafe
            {
                // Get a pointer to the back buffer. 
                float* pBackBuffer = (float*)_writeableBitmap.BackBuffer;

                for (int rowIdx = 0; rowIdx < pixelHeightInt; rowIdx++)
                {
                    float* row = pBackBuffer + rowIdx * _writeableBitmap.BackBufferStride / sizeof(float);
                    for (int i = 0; i < pixelWidthInt; i++)
                    {
                        *row = (elevations[rowIdx][i] - stats.Min) / stats.Range;
                        row++;
                    }
                }
            }

            // Specify the area of the bitmap that changed.
            _writeableBitmap.AddDirtyRect(new Int32Rect(0, 0, pixelWidthInt, _writeableBitmap.PixelHeight));

            // Release the back buffer and make it available for display.
            _writeableBitmap.Unlock();


            this.MinElevationLabel.Content = stats.Min;
            this.MaxElevationLabel.Content = stats.Max;
            this.ElevationRangeLabel.Content = stats.Range;
            this.AverageElevationLabel.Content = stats.Average;
        }

        private static Task<ElevationValueStats> elevationStatsAsync(IEnumerable<float> allElevations)
        {
            return Task.Run(() => elevationStatsAsyncImpl(allElevations));
        }

        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        private static ElevationValueStats elevationStatsAsyncImpl(IEnumerable<float> allElevationsLinear)
        {
            var allElevations = allElevationsLinear.AsParallel();
            float elevationMax = allElevations.Max();
            float elevationMin = allElevations.Min();

            float elevationDiff = elevationMax - elevationMin;
            float elevationAvg = allElevations.Average();
            return new ElevationValueStats(elevationMin, elevationMax, elevationDiff, elevationAvg);
        }

        private static Task<float[][]> retrieveElevationAsync(
            IElevationProvider ep,
            int pixelHeightInt,
            int pixelWidthInt,
            double latCenter,
            double lonCenter,
            double range)
        {
            return Task.Run(() => retrieveElevationAsyncImpl(ep, pixelHeightInt, pixelWidthInt, latCenter, lonCenter, range));
        }

        private static float[][] retrieveElevationAsyncImpl(
            IElevationProvider ep,
            int pixelHeightInt,
            int pixelWidthInt,
            double latCenter,
            double lonCenter,
            double range)
        {
            double minLat = Math.Max(latCenter - range, -90);
            double maxLat = Math.Min(latCenter + range, 90);

            double minLon = Math.Max(lonCenter - range, -180);
            double maxLon = Math.Min(lonCenter + range, 180);

            double latRange = maxLat - minLat;
            double lonRange = maxLon - minLon;

            var rowIndexes = Enumerable.Range(0, pixelHeightInt);

            return rowIndexes
                .AsParallel().AsOrdered()
                .Select(rIdx =>
                        {
                            double lat = minLat + rIdx * latRange / pixelHeightInt;
                            return retrieveRowElevation(ep, pixelWidthInt, lat, minLon, lonRange);
                        })
                .ToArray();
        }

        private static float[] retrieveRowElevation(IElevationProvider ep, int pixelWidthInt, double lat, double minLon, double lonRange)
        {
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

    internal struct ElevationValueStats
    {
        public readonly float Min;
        public readonly float Max;
        public readonly float Range;
        public readonly float Average;

        public ElevationValueStats(float min, float max, float range, float average)
        {
            Min = min;
            Max = max;
            Range = range;
            Average = average;
        }
    }
}
