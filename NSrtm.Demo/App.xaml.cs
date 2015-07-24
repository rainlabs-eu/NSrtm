using System;
using System.Collections.Specialized;
using System.Runtime.Caching;
using System.Windows;
using NSrtm.Core;

namespace NSrtm.Demo
{
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
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
