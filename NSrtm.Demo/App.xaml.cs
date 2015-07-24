using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using NSrtm.Core;

namespace NSrtm.Demo
{

    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var mainWindow = new MainWindow();
            mainWindow.ElevationProviders = new IElevationProvider[]
                                             {
                                                 new HgtData("D:\\SRTM3"),
                                             };
            this.MainWindow = mainWindow;
            this.MainWindow.Show();
        }
    }
}
