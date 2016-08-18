using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using CapsCollection.Business.DTOs;

namespace CapsCollection.Web.ServiceHost.Contracts
{
    [ServiceContract]
    public interface IBeerService
    {
        [OperationContract]
        List<BreweryDto> GetBreweriesByCity(int cityId);
        [OperationContract]
        [WebInvoke(UriTemplate = "/GetBreweriesByFilter?continentId={continentId}&countryId={countryId}&regionId={regionId}&cityId={cityId}", Method = "GET", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        List<BreweryDto> GetBreweriesByFilter(int continentId, int countryId, int regionId, int cityId);
        [OperationContract]
        List<BreweryDto> GetBreweries();
        [OperationContract]
        BreweryDto GetBrewery(int breweryId);
        [OperationContract]
        void UpdateBrewery(BreweryDto brewery);
        [OperationContract]
        void DeleteBrewery(BreweryDto brewery);


        [OperationContract]
        [WebInvoke(UriTemplate = "/GetAllBeers", Method = "GET", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        List<BeerDto> GetAllBeers();
        [OperationContract]
        [WebInvoke(UriTemplate = "/GetCountryBeers?countryId={countryId}", Method = "GET", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        List<BeerDto> GetCountryBeers(int countryId);
        [OperationContract]
        [WebInvoke(UriTemplate = "/GetBeerByName?beerName={beerName}", Method = "GET", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        List<BeerDto> GetBeerByName(string beerName);
        [OperationContract]
        [WebInvoke(UriTemplate = "/GetBeer?beerId={beerId}", Method = "GET", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        BeerDto GetBeer(int beerId);
        [OperationContract]
        int UpdateBeer(BeerDto beer);
        [OperationContract]
        void DeleteBeer(BeerDto beer);
        [OperationContract]
        List<BreweryDto> GetBreweriesByCountry(int countryId);


        [OperationContract]
        List<BeerStyleDto> GetBeerStyles();
        [OperationContract]
        BeerStyleDto GetBeerStyle(int beerStyleId);
        [OperationContract]
        void UpdateBeerStyle(BeerStyleDto beerStyle);
        [OperationContract]
        void DeleteBeerStyle(BeerStyleDto beerStyle);


        [OperationContract]
        List<CapTypeDto> GetCapTypes();


        [OperationContract]
        void ProcessImageFiles(IList<ImageFileOperationDto> imageFiles);
        [OperationContract]
        void DeleteAllFilesFromContainer(string imageContainer);
        [OperationContract]
        [WebInvoke(UriTemplate = "DownloadImageFile", Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped)]
        byte[] DownloadImageFile(string container, string fileName);
    }
}
