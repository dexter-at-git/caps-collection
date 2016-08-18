using CapsCollection.Desktop.Infrastructure.Resources;
using CapsCollection.Desktop.UI.Modules.Services;
using CapsCollection.Desktop.UI.Modules.Services.Interfaces;
using CapsCollection.Desktop.UI.Modules.Settings.ViewModels;
using CapsCollection.Desktop.UI.Modules.Settings.Views;
using FluentValidation;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;

namespace CapsCollection.Desktop.UI.Modules.Settings
{
    public class SettingsModule : IModule
    {
        private readonly IUnityContainer _container;
        private readonly IRegionManager _regionManager;

        public SettingsModule(IUnityContainer container, IRegionManager regionManager)
        {
            _container = container;
            _regionManager = regionManager;
        }
       
        public void Initialize()
        {
            _container.RegisterType<IBeerServiceRepository, BeerRepository>();

            _container.RegisterType<ISettingsView, SettingsView>();
            _container.RegisterType<ISettingsViewModel, SettingsViewModel>();

            _container.RegisterType<IValidator<ImageLookupFolders>, ImageLookupFoldersValidator>(new ContainerControlledLifetimeManager());

            var viewModel = _container.Resolve<ISettingsViewModel>();
            IRegion contentRegion = _regionManager.Regions[RegionNames.ContentRegion];
            contentRegion.Add(viewModel.View);
        }
    }
}
