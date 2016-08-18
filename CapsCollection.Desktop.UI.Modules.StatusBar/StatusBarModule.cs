using CapsCollection.Desktop.Infrastructure.Resources;
using CapsCollection.Desktop.UI.Modules.Services;
using CapsCollection.Desktop.UI.Modules.Services.Interfaces;
using CapsCollection.Desktop.UI.Modules.StatusBar.ViewModels;
using CapsCollection.Desktop.UI.Modules.StatusBar.Views;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;

namespace CapsCollection.Desktop.UI.Modules.StatusBar
{
    public class StatusBarModule : IModule
    {
        private IUnityContainer _container;
        private IRegionManager _regionManager;

        public StatusBarModule(IUnityContainer container, IRegionManager regionManager)
        {
            _container = container;
            _regionManager = regionManager;
        }
       
        public void Initialize()
        {
            _container.RegisterType<IBeerServiceRepository, BeerRepository>();

            _container.RegisterType<IStatusBarView, StatusBarView>();
            _container.RegisterType<IStatusBarViewModel, StatusBarViewModel>();

            var viewModel = _container.Resolve<IStatusBarViewModel>();
            IRegion statusBarRegion = _regionManager.Regions[RegionNames.StatusBarRegion];
            statusBarRegion.Add(viewModel.View);
        }
    }
}
