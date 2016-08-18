using System;
using System.Windows;
using CapsCollection.Desktop.Infrastructure.Resources;
using CapsCollection.Desktop.UI.Modules.BulkLoad;
using CapsCollection.Desktop.UI.Modules.BulkUpdate;
using CapsCollection.Desktop.UI.Modules.Home;
using CapsCollection.Desktop.UI.Modules.Services;
using CapsCollection.Desktop.UI.Modules.Settings;
using CapsCollection.Desktop.UI.Modules.StatusBar;
using CapsCollection.Desktop.UI.Shell.Views;
using FluentValidation;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;
using Prism.Unity;

namespace CapsCollection.Desktop.UI.Shell.Bootstrapper
{
    public class BootstrapperUnity : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            this.Container.RegisterType<IValidatorFactory, UnityValidatorFactory>(new ContainerControlledLifetimeManager());

            return ServiceLocator.Current.GetInstance<UnityShell>();
        }

        protected override void InitializeShell()
        {
            IRegionManager regionManager = ServiceLocator.Current.GetInstance<IRegionManager>();
            regionManager.RegisterViewWithRegion(RegionNames.ContentRegion, typeof(TabsView));

            Application.Current.MainWindow = (UnityShell)Shell;
            Application.Current.MainWindow.Show();
        }

        protected override void ConfigureModuleCatalog()
        {
            Type moduleServiceType = typeof(ServicesModule);
            ModuleCatalog.AddModule(new ModuleInfo
            {
                ModuleName = moduleServiceType.Name,
                ModuleType = moduleServiceType.AssemblyQualifiedName,
                InitializationMode = InitializationMode.WhenAvailable
            });

            Type moduleHomeType = typeof(HomeModule);
            ModuleCatalog.AddModule(new ModuleInfo
            {
                ModuleName = moduleHomeType.Name,
                ModuleType = moduleHomeType.AssemblyQualifiedName,
                InitializationMode = InitializationMode.WhenAvailable
            });

            Type moduleBulkLoadType = typeof(BulkLoadModule);
            ModuleCatalog.AddModule(new ModuleInfo
            {
                ModuleName = moduleBulkLoadType.Name,
                ModuleType = moduleBulkLoadType.AssemblyQualifiedName,
                InitializationMode = InitializationMode.WhenAvailable
            });

            Type moduleBulkUpdateType = typeof(BulkUpdateModule);
            ModuleCatalog.AddModule(new ModuleInfo
            {
                ModuleName = moduleBulkUpdateType.Name,
                ModuleType = moduleBulkUpdateType.AssemblyQualifiedName,
                InitializationMode = InitializationMode.WhenAvailable
            });

            Type moduleStatusBarType = typeof(StatusBarModule);
            ModuleCatalog.AddModule(new ModuleInfo
            {
                ModuleName = moduleStatusBarType.Name,
                ModuleType = moduleStatusBarType.AssemblyQualifiedName,
                InitializationMode = InitializationMode.WhenAvailable
            });

            Type moduleSettingsType = typeof(SettingsModule);
            ModuleCatalog.AddModule(new ModuleInfo
            {
                ModuleName = moduleSettingsType.Name,
                ModuleType = moduleSettingsType.AssemblyQualifiedName,
                InitializationMode = InitializationMode.WhenAvailable
            });
        }
    }
}
