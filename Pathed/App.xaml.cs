using Pathed.Services;
using Pathed.ViewModels;
using Pathed.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace Pathed
{
    public partial class App : Application
    {
        private AggregateCatalog catalog;
        private CompositionContainer container;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                this.catalog = new AggregateCatalog(new AssemblyCatalog(Assembly.GetExecutingAssembly()));
                this.container = new CompositionContainer(this.catalog);

                Application.Current.MainWindow = this.container.GetExportedValue<ShellView>();
                Application.Current.MainWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                Application.Current.Shutdown(1);
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            try
            {
                this.container.Dispose();
                this.catalog.Dispose();
            }
            catch { }
        }
    }
}
