using AutoMapper;
using CapsCollection.Business.BuisenessServices;
using CapsCollection.Business.BuisenessServices.Interfaces;
using CapsCollection.Business.DTOs.MapperProfiles;
using CapsCollection.Data.Repositories;
using CapsCollection.Data.Repositories.Interfaces;
using CapsCollection.Web.ServiceHost.Contracts;
using CapsCollection.Web.ServiceHost.Implementations;
using Microsoft.Practices.Unity;

namespace CapsCollection.Web.ServiceHost.ServiceBehaviors
{
    public static class UnityContainer
    {
        #region Properties

        private static IUnityContainer _currentContainer;

        public static IUnityContainer Current
        {
            get
            {
                return _currentContainer;
            }
        }

        #endregion


        #region Constructor

        static UnityContainer()
        {
            ConfigureContainer();
        }

        #endregion


        #region Methods

        static void ConfigureContainer()
        {
            _currentContainer = new Microsoft.Practices.Unity.UnityContainer();

            // WCF services
            _currentContainer.RegisterType<IGeographyService, GeographyService>();
            _currentContainer.RegisterType<IBeerService, BeerService>();
            _currentContainer.RegisterType<IAuthenticationService, AuthenticationService>();

            // Buiseness services
            _currentContainer.RegisterType<IAuthenticationBuisenessService, AuthenticationBuisenessService>();
            _currentContainer.RegisterType<IBreweryBuisenessService, BreweryBuisenessService>();
            _currentContainer.RegisterType<ICollectionBuisenessService, CollectionBuisenessService>();
            _currentContainer.RegisterType<IBeerStyleBuisenessService, BeerStyleBuisenessService>();
            _currentContainer.RegisterType<ICountryBuisenessService, CountryBuisenessService>();
            _currentContainer.RegisterType<IRegionBuisenessService, RegionBuisenessService>();
            _currentContainer.RegisterType<ICityBuisenessService, CityBuisenessService>();
            _currentContainer.RegisterType<IUserSecurityService, UserSecurityService>();

            // Repositories
            _currentContainer.RegisterType<IContinentRepository, ContinentRepository>();
            _currentContainer.RegisterType<ICountryRepository, CountryRepository>();
            _currentContainer.RegisterType<IRegionRepository, RegionRepository>();
            _currentContainer.RegisterType<ICityRepository, CityRepository>();
            _currentContainer.RegisterType<IImageFileRepository, ImageFileBlobRepository>();
            _currentContainer.RegisterType<IBreweryRepository, BreweryRepository>();
            _currentContainer.RegisterType<ICollectionRepository, CollectionRepository>();
            _currentContainer.RegisterType<IBeerStyleRepository, BeerStyleRepository>();
            _currentContainer.RegisterType<IBeerCapTypeRepository, BeerCapTypeRepository>();
            _currentContainer.RegisterType<IUserRepository, UserRepository>();

            Mapper.Initialize(mapperConfiguration =>
            {
                mapperConfiguration.AddProfile(new BeerMapperProfile());
                mapperConfiguration.AddProfile(new GeographyMapperProfile());

            });
        }

        #endregion
    }
}
