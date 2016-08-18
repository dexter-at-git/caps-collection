using CapsCollection.Desktop.Infrastructure.Resources;
using CapsCollection.Desktop.UI.Modules.Home.Validators;
using CapsCollection.Desktop.UI.Modules.Home.ViewModels;
using CapsCollection.Desktop.UI.Modules.Home.Views;
using CapsCollection.Desktop.UI.Modules.Services;
using CapsCollection.Desktop.UI.Modules.Services.Interfaces;
using FluentValidation;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;

namespace CapsCollection.Desktop.UI.Modules.Home
{
    public class HomeModule : IModule
    {
        private readonly IUnityContainer _container;
        private readonly IRegionManager _regionManager;

        public HomeModule(IUnityContainer container, IRegionManager regionManager)
        {
            _container = container;
            _regionManager = regionManager;
        }
       
        public void Initialize()
        {
            _container.RegisterType<IBeerServiceRepository, BeerRepository>();

            _container.RegisterType<IHomeView, HomeView>();
            _container.RegisterType<IHomeViewModel, HomeViewModel>();
            
            _container.RegisterType<IImageTypeAggregator, ImageTypeAggregator>();
            _container.RegisterType<IValidator<HomeViewModel>, HomeViewModelValidator>(new ContainerControlledLifetimeManager());

            var viewModel = _container.Resolve<IHomeViewModel>();
            IRegion contentRegion = _regionManager.Regions[RegionNames.TabRegion];
            contentRegion.Add(viewModel.View);
        }
    }
}
