using System;
using System.Collections.Generic;
using System.ServiceModel;
using CapsCollection.Business.BuisenessServices.Interfaces;
using CapsCollection.Business.DTOs;
using CapsCollection.Web.ServiceHost.Contracts;
using CapsCollection.Web.ServiceHost.ServiceBehaviors;

namespace CapsCollection.Web.ServiceHost.Implementations
{
    [UnityInstanceProviderServiceBehavior()]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class GeographyService : IGeographyService
    {
        #region Members

        private readonly ICountryBuisenessService _countryBuisenessService;
        private readonly IRegionBuisenessService _regionBuisenessService;
        private readonly ICityBuisenessService _cityBuisenessService;
        private readonly ICollectionBuisenessService _collectionBuisenessService;

        #endregion


        #region Constructor

        public GeographyService(ICountryBuisenessService countryBuisenessService, IRegionBuisenessService regionBuisenessService, ICityBuisenessService cityBuisenessService, ICollectionBuisenessService collectionBuisenessService)
        {
            if (countryBuisenessService == null)
                throw new ArgumentNullException("countryBuisenessService");
            if (regionBuisenessService == null)
                throw new ArgumentNullException("regionBuisenessService");
            if (cityBuisenessService == null)
                throw new ArgumentNullException("cityBuisenessService");

            _countryBuisenessService = countryBuisenessService;
            _regionBuisenessService = regionBuisenessService;
            _cityBuisenessService = cityBuisenessService;
            _collectionBuisenessService = collectionBuisenessService;
        }

        #endregion


        #region Implementation

        public List<ContinentDto> GetContinents()
        {
            return _countryBuisenessService.GetContinents();
        }

        public List<CountryDto> GetAllCountries()
        {
            return _countryBuisenessService.GetAllCountries();
        }

        public List<CountryDto> GetCountriesByContinent(int continentId, bool includeFlags)
        {
            return _countryBuisenessService.GetCountriesByContinent(continentId, includeFlags);
        }

        public CountryDto GetCountry(int countryId)
        {
            return _countryBuisenessService.GetCountry(countryId);
        }

        public void UpdateCountry(CountryDto country, IEnumerable<ImageFileOperationDto> flags)
        {
            _countryBuisenessService.SaveCountry(country, flags);
        }

        public void DeleteCountry(CountryDto country)
        {
            _countryBuisenessService.DeleteCountry(country);
        }

        public List<RegionDto> GetAllRegions()
        {
            return _regionBuisenessService.GetAllRegions();
        }

        public List<RegionDto> GetRegionsByCountry(int countryId)
        {
            return _regionBuisenessService.GetRegionsByCountry(countryId);
        }

        public RegionDto GetRegion(int regionId)
        {
            return _regionBuisenessService.GetRegion(regionId);
        }

        public void UpdateRegion(RegionDto region)
        {
            _regionBuisenessService.SaveRegion(region);
        }

        public void DeleteRegion(RegionDto region)
        {
            _regionBuisenessService.DeleteRegion(region);
        }

        public List<CityDto> GetAlCities()
        {
            return _cityBuisenessService.GetAllCities();
        }

        public List<CityDto> GetCitiesByRegion(int regionId)
        {
            return _cityBuisenessService.GetCitiesByRegion(regionId);
        }

        public CityDto GetCity(int cityId)
        {
            return _cityBuisenessService.GetCity(cityId);
        }

        public void UpdateCity(CityDto city)
        {
            _cityBuisenessService.SaveCity(city);
        }

        public void DeleteCity(CityDto city)
        {
            _cityBuisenessService.DeleteCity(city);
        }

        public List<BeerCountryDto> GetBeerCountries()
        {
            return _collectionBuisenessService.GetBeerCountries();
        }

        public List<ContinentDto> GetContinentsWithBreweries()
        {
            return _collectionBuisenessService.GetContinentsWithBreweries();
        }

        public List<CountryDto> GetContinentCountriesWithBreweries(int continentId)
        {
            return _collectionBuisenessService.GetContinentCountriesWithBreweries(continentId);
        }

        public List<CountryDto> GetCountriesWithBreweries()
        {
            return _collectionBuisenessService.GetCountriesWithBreweries();
        }

        #endregion
    }
}
