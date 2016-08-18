using Microsoft.Practices.Prism.MefExtensions;
using Microsoft.Practices.Prism.Modularity;
using System;
using System.ComponentModel.Composition.Hosting;
using System.Windows;

namespace CapsCollection.Silverlight.UI.Shell
{
    public class Bootstrapper : MefBootstrapper
    {
        protected override void ConfigureAggregateCatalog()
        {
            base.ConfigureAggregateCatalog();
            AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(Bootstrapper).Assembly));
        }

        protected override DependencyObject CreateShell()
        {
            Shell shell = Container.GetExportedValue<Shell>();
            Application.Current.RootVisual = shell;
            return shell;
        }

        protected override IModuleCatalog CreateModuleCatalog()
        {
            var catalog = Microsoft.Practices.Prism.Modularity.ModuleCatalog.CreateFromXaml(
               new Uri("/CapsCollection.Silverlight.UI.Shell;component/ModuleCatalog.xaml", UriKind.Relative));
            return catalog;
        }

        protected override CompositionContainer CreateContainer()
        {
            var container = base.CreateContainer();
            CompositionHost.Initialize(container);
            return container;
        }
    }
}
