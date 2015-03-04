using System;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using System.Windows;
using Pathed.Views;

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
