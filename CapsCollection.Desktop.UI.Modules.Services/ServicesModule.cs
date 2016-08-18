using CapsCollection.Desktop.UI.Modules.Services.Interfaces;
using Microsoft.Practices.Unity;
using Prism.Modularity;

namespace CapsCollection.Desktop.UI.Modules.Services
{
    public class ServicesModule : IModule
    {
        private readonly IUnityContainer _container;

        public ServicesModule(IUnityContainer container)
        {
            _container = container;
        }

        public void Initialize()
        {
            _container.RegisterType<IBeerServiceRepository, BeerRepository>(new ContainerControlledLifetimeManager());
            _container.RegisterType<IFileRepository, FileRepository>(new ContainerControlledLifetimeManager());
            _container.RegisterType<IThumbnailService, ThumbnailService>(new ContainerControlledLifetimeManager());
            _container.RegisterType<IEventProcessingService, EventProcessingService>(new ContainerControlledLifetimeManager());
            _container.RegisterType<IFileSystem, FileSystem>(new ContainerControlledLifetimeManager());
            _container.RegisterType<IImageProcessingService, ImageProcessingService>(new ContainerControlledLifetimeManager());
            _container.Resolve<IEventProcessingService>();
        }
    }
}
