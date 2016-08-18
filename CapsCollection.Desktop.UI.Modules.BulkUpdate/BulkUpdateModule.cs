using CapsCollection.Desktop.Infrastructure.Resources;
using CapsCollection.Desktop.UI.Modules.BulkUpdate.Validators;
using CapsCollection.Desktop.UI.Modules.BulkUpdate.ViewModels;
using CapsCollection.Desktop.UI.Modules.BulkUpdate.Views;
using FluentValidation;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;

namespace CapsCollection.Desktop.UI.Modules.BulkUpdate
{
    public class BulkUpdateModule : IModule
    {
        private readonly IUnityContainer _container;
        private readonly IRegionManager _regionManager;

        public BulkUpdateModule(IUnityContainer container, IRegionManager regionManager)
        {
            _container = container;
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            _container.RegisterType<IBeerUpdateView, BeerUpdateView>();
            _container.RegisterType<IBeerUpdateViewModel, BeerUpdateViewModel>();

            _container.RegisterType<IBulkUpdateView, BulkUpdateView>();
            _container.RegisterType<IBulkUpdateViewModel, BulkUpdateViewModel>();

            _container.RegisterType<IValidator<BeerUpdateViewModel>, BeerUpdateViewModelValidator>(new ContainerControlledLifetimeManager());

            var viewModel = _container.Resolve<IBulkUpdateViewModel>();
            IRegion contentRegion = _regionManager.Regions[RegionNames.TabRegion];
            contentRegion.Add(viewModel.View);
        }
    }
}
