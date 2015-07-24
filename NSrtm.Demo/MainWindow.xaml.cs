using System;
using System.Collections.Generic;
using System.Linq;
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
            _writeableBitmap = new WriteableBitmap(800, 600, 96, 96, PixelFormats.Gray32Float, null);
            ElevationImage.Source = _writeableBitmap;
        }

        public IEnumerable<IElevationProvider> ElevationProviders
        {
            get { return ElevationModeCombo.ItemsSource as IEnumerable<IElevationProvider>; }
            set { ElevationModeCombo.ItemsSource = value; }
        }

        private void ElevationImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            double latCenter = LatitudeSlider.Value;
            double range = AreaSlider.Value;
            double lonCenter = LongitudeSlider.Value;

            double minLat = Math.Max(latCenter - range, -90);
            double maxLat = Math.Min(latCenter + range, 90);
            double latRange = maxLat - minLat;

            double minLon = Math.Max(lonCenter - range, -180);
            double maxLon = Math.Min(lonCenter + range, 180);
            double lonRange = maxLon - minLon;

            var ep = ElevationModeCombo.SelectedItem as IElevationProvider;
            if (ep == null)
            {
                return;
            }

            int pixelWidthInt = _writeableBitmap.PixelWidth;
            int pixelHeightInt = _writeableBitmap.PixelHeight;
            float pixelWidth = pixelWidthInt;
            float pixelHeight = pixelHeightInt;

            var elevations = new float[pixelHeightInt][];
            for (int rIdx = 0; rIdx < pixelHeightInt; rIdx++)
            {
                elevations[rIdx] = new float[pixelWidthInt];
                for (int cIdx = 0; cIdx < pixelWidthInt; cIdx++)
                {
                    double lat = minLat + rIdx / pixelHeight * latRange;
                    double lon = minLon + cIdx / pixelWidth * lonRange;
                    float elevation = (float)ep.GetElevation(lat, lon);
                    elevations[rIdx][cIdx] = elevation;
                }
            }

            var elevationMax = elevations.SelectMany(row => row)
                      .Max();

            var elevationMin = elevations.SelectMany(row => row)
                      .Min();

            var elevationRange = elevationMax - elevationMin;


            // Reserve the back buffer for updates.
            _writeableBitmap.Lock();

            unsafe
            {
                // Get a pointer to the back buffer. 
                float* pBackBuffer = (float*)_writeableBitmap.BackBuffer;

                for (int rowIdx = 0; rowIdx < pixelHeightInt; rowIdx++)
                {
                    float* row = pBackBuffer + rowIdx * _writeableBitmap.BackBufferStride/sizeof(float);
                    for (int i = 0; i < pixelWidthInt; i++)
                    {
                        *row = (elevations[rowIdx][i] - elevationMin)/elevationRange;
                        row++;
                    }
                }
            }

            // Specify the area of the bitmap that changed.
            _writeableBitmap.AddDirtyRect(new Int32Rect(0, 0, pixelWidthInt, _writeableBitmap.PixelHeight));

            // Release the back buffer and make it available for display.
            _writeableBitmap.Unlock();
        }
    }
}
