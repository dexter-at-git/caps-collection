using CapsCollection.Desktop.Infrastructure.Resources;
using CapsCollection.Desktop.UI.Modules.BulkLoad.Validators;
using CapsCollection.Desktop.UI.Modules.BulkLoad.ViewModels;
using CapsCollection.Desktop.UI.Modules.BulkLoad.Views;
using FluentValidation;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;

namespace CapsCollection.Desktop.UI.Modules.BulkLoad
{
    public class BulkLoadModule : IModule
    {
        private readonly IUnityContainer _container;
        private readonly IRegionManager _regionManager;

        public BulkLoadModule(IUnityContainer container, IRegionManager regionManager)
        {
            _container = container;
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            _container.RegisterType<IBulkLoadView, BulkLoadView>();
            _container.RegisterType<IBulkLoadViewModel, BulkLoadViewModel>();

            _container.RegisterType<IBeerLoadView, BeerLoadView>();
            _container.RegisterType<IBeerLoadViewModel, BeerLoadViewModel>();

            _container.RegisterType<IValidator<BeerLoadViewModel>, BeerLoadViewModelValidator>(new ContainerControlledLifetimeManager());

            var viewModel = _container.Resolve<IBulkLoadViewModel>();
            IRegion contentRegion = _regionManager.Regions[RegionNames.TabRegion];
            contentRegion.Add(viewModel.View);
        }
    }
}
