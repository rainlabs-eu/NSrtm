using System;
using System.Windows;
using JetBrains.Annotations;
using NSrtm.Core;

namespace NSrtm.Demo
{
    internal partial class App
    {
        protected override void OnStartup([NotNull] StartupEventArgs e)
        {
            if (e == null) throw new ArgumentNullException("e");

            base.OnStartup(e);
            var mainWindow = new MainWindow();
            MainWindow = mainWindow;
            MainWindow.Show();
            mainWindow.ViewModel = new DemoViewModel(new IElevationProvider[]
                                                     {
                                                         HgtElevationProvider.CreateInMemoryFromZipFiles("D:\\SRTM3ZIP"),
                                                         HgtElevationProvider.CreateInMemoryFromRawFiles("D:\\SRTM3HGT"),
                                                         HgtElevationProvider.CreateMemoryMappedFromRawFiles("D:\\SRTM3HGT"),
                                                     });
        }
    }
}
