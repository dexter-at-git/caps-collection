using System.Collections.Generic;
using CapsCollection.Business.DTOs;

namespace CapsCollection.Business.BuisenessServices.Interfaces
{
    public interface ICollectionBuisenessService
    {
        void ProcessImageFiles(IList<ImageFileOperationDto> imageFiles);

        void DeleteAllFilesFromContainer(string imageContainer);

        byte[] DownloadImageFile(string container, string fileName);

        List<BeerCountryDto> GetBeerCountries();

        List<BeerDto> GetAllBeers();

        List<BeerDto> GetCountryBeers(int countryId);

        BeerDto GetBeer(int beerId);

        int SaveBeer(BeerDto beer);

        void DeleteBeer(BeerDto beer);

        List<BreweryDto> GetBreweriesByCountry(int countryId);

        List<ContinentDto> GetContinentsWithBreweries();

        List<CountryDto> GetContinentCountriesWithBreweries(int continentId);

        List<CountryDto> GetCountriesWithBreweries();

        List<CapTypeDto> GetCapTypes();
        List<BeerDto> GetBeerByName(string beerName);
    }
}
