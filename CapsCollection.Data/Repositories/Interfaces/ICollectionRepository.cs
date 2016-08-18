using System.Collections.Generic;
using CapsCollection.Data.Models;

namespace CapsCollection.Data.Repositories.Interfaces
{
    public interface ICollectionRepository : IGenericRepository<Beer_Beer>
    {
        IEnumerable<Geography_Continent> GetContinentsWithBreweries();
        IEnumerable<Geography_Country> GetContinentCountriesWithBreweries(int continentId);
        IEnumerable<Geography_Country> GetCountriesWithBreweries();
        IEnumerable<Geography_Country> GetBeerCountries();
        IEnumerable<Beer_Beer> GetAllBeers();
        IEnumerable<Beer_Beer> GetCountryBeers(int countryId);
        IEnumerable<Beer_Brewery> GetBreweriesByCountry(int countryId);
        IEnumerable<Beer_Beer> GetBeerByName(string beerName);
        Beer_Beer GetBeer(int beerId);
    }
}
