using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using CapsCollection.Common.Extensions;
using CapsCollection.Data.DataContext;
using CapsCollection.Data.Models;
using CapsCollection.Data.Repositories.Interfaces;

namespace CapsCollection.Data.Repositories
{
    public class CollectionRepository : GenericRepository<Beer_Beer>, ICollectionRepository
    {
        private readonly CapsCollectionContext _context;

        public CollectionRepository(CapsCollectionContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<Geography_Continent> GetContinentsWithBreweries()
        {
            var continents = from brewery in _context.Breweries
                             join city in _context.Cities on brewery.CityId equals city.CityID
                             join region in _context.Regions on city.RegionID equals region.RegionID
                             join country in _context.Countries on region.CountryID equals country.CountryID
                             join continent in _context.Continents on country.ContinentID equals continent.ContinentID
                             group continent by new { continent } into grp
                             select grp.Key.continent;

            return continents;
        }

        public IEnumerable<Geography_Country> GetContinentCountriesWithBreweries(int continentId)
        {
            var countries = from brewery in _context.Breweries
                            join city in _context.Cities on brewery.CityId equals city.CityID
                            join region in _context.Regions on city.RegionID equals region.RegionID
                            join country in _context.Countries on region.CountryID equals country.CountryID
                            where country.ContinentID == continentId
                            group country by new { country } into grp
                            select grp.Key.country;

            return countries;
        }

        public IEnumerable<Geography_Country> GetCountriesWithBreweries()
        {
            var countries = from brewery in _context.Breweries
                            join city in _context.Cities on brewery.CityId equals city.CityID
                            join region in _context.Regions on city.RegionID equals region.RegionID
                            join country in _context.Countries on region.CountryID equals country.CountryID
                            group country by new { country } into grp
                            select grp.Key.country;

            return countries.Include(x => x.Continent);
        }

        public IEnumerable<Geography_Country> GetBeerCountries()
        {
            var beerCountries = from country in _context.Countries
                                join beer in _context.Beers on country.CountryID equals beer.CountryId
                                group country by new { country } into grp
                                select grp.Key.country;

            return beerCountries.Include(x => x.Continent);
        }

        public IEnumerable<Beer_Beer> GetAllBeers()
        {
            return _context.Beers.Include(x => x.Country).Include(x => x.Country.Continent);
        }

        public IEnumerable<Beer_Beer> GetCountryBeers(int countryId)
        {
            return _context.Beers.Include(x => x.Country).Include(x => x.Country.Continent).Where(c => c.CountryId == countryId);
        }

        public IEnumerable<Beer_Brewery> GetBreweriesByCountry(int countryId)
        {
            var breweries = from brewery in _context.Breweries
                            join city in _context.Cities on brewery.CityId equals city.CityID
                            join region in _context.Regions on city.RegionID equals region.RegionID
                            join country in _context.Countries on region.CountryID equals country.CountryID
                            where country.CountryID == countryId
                            select brewery;

            return breweries.Include(x => x.City).Include(x => x.City.Region).Include(x => x.City.Region.Country).Include(x => x.City.Region.Country.Continent);
        }

        public IEnumerable<Beer_Beer> GetBeerByName(string beerName)
        {
            return _context.Beers.ToList().Where(c => c.BeerName.ToLower().RemoveDiacritics().Contains(beerName.ToLower()));
        }

        public Beer_Beer GetBeer(int beerId)
        {
            return _context.Beers.Include(x => x.Country).Include(x => x.Country.Continent).FirstOrDefault(x => x.BeerId == beerId);
        }
    }
}
