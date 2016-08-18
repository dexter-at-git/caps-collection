using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using CapsCollection.Business.DTOs;

namespace CapsCollection.Web.ServiceHost.Contracts
{
    [ServiceContract]
    public interface IGeographyService
    {
        [OperationContract]
        [WebInvoke(UriTemplate = "/GetContinents", Method = "GET", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        List<ContinentDto> GetContinents();


        [OperationContract]
        List<CountryDto> GetAllCountries();

        [OperationContract]
        [WebGet(UriTemplate = "/GetCountriesByContinent?continentId={continentId}&includeFlags={includeFlags}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<CountryDto> GetCountriesByContinent(int continentId, bool includeFlags);

        [OperationContract]
        [WebInvoke(UriTemplate = "/GetCountry?countryId={countryId}", Method = "GET", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        CountryDto GetCountry(int countryId);

        [OperationContract]
        [WebInvoke(UriTemplate = "UpdateCountry", Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped)]
        void UpdateCountry(CountryDto country, IEnumerable<ImageFileOperationDto> flags);

        [OperationContract]
        void DeleteCountry(CountryDto country);


        [OperationContract]
        List<RegionDto> GetAllRegions();

        [OperationContract]
        List<RegionDto> GetRegionsByCountry(int countryId);

        [OperationContract]
        RegionDto GetRegion(int regionId);

        [OperationContract]
        void UpdateRegion(RegionDto region);

        [OperationContract]
        void DeleteRegion(RegionDto region);


        [OperationContract]
        List<CityDto> GetAlCities();

        [OperationContract]
        List<CityDto> GetCitiesByRegion(int regionId);

        [OperationContract]
        CityDto GetCity(int cityId);

        [OperationContract]
        void UpdateCity(CityDto city);

        [OperationContract]
        void DeleteCity(CityDto city);


        [OperationContract]
        [WebInvoke(UriTemplate = "/GetBeerCountries", Method = "GET", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        List<BeerCountryDto> GetBeerCountries();
        [OperationContract]
        List<ContinentDto> GetContinentsWithBreweries();

        [OperationContract]
        List<CountryDto> GetContinentCountriesWithBreweries(int continentId);

        [OperationContract]
        List<CountryDto> GetCountriesWithBreweries();
    }
}
