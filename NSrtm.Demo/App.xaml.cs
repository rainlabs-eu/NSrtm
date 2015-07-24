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
            var mainWindow = new MainWindow
                             {
                                 ElevationProviders = new IElevationProvider[]
                                                      {
                                                          new HgtData("D:\\SRTM3"),
                                                      }
                             };
            MainWindow = mainWindow;
            MainWindow.Show();
        }
    }
}
