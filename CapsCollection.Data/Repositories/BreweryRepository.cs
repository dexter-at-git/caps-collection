using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using CapsCollection.Data.DataContext;
using CapsCollection.Data.Models;
using CapsCollection.Data.Repositories.Interfaces;

namespace CapsCollection.Data.Repositories
{
    public class BreweryRepository : GenericRepository<Beer_Brewery>, IBreweryRepository
    {
        private readonly CapsCollectionContext _context;

        public BreweryRepository(CapsCollectionContext context) : base(context)
        {
            _context = context;
        }


        public IEnumerable<Beer_Brewery> GetAllBreweries()
        {
            return _context.Breweries.Include(x => x.City.Region.Country.Continent);
        }

        public IEnumerable<Beer_Brewery> GetBreweriesByCity(int cityId)
        {
            return _context.Breweries.Include(x => x.City.Region.Country.Continent).Where(c => c.CityId == cityId);
        }

        public IEnumerable<Beer_Brewery> GetBreweriesByFilter(int continentId, int countryId, int regionId, int cityId)
        {
            var geography = from continent in _context.Continents
                            join country in _context.Countries on continent.ContinentID equals country.ContinentID
                            join region in _context.Regions on country.CountryID equals region.CountryID
                            join city in _context.Cities on region.RegionID equals city.RegionID
                            select new { ContinentId = continent.ContinentID, CountryId = country.CountryID, RegionId = region.RegionID, CityId = city.CityID };

            var filteredGeography = geography;
            if (continentId != -1)
                filteredGeography = filteredGeography.Where(c => c.ContinentId == continentId).OrderBy(c => c.ContinentId);
            if (countryId != -1)
                filteredGeography = filteredGeography.Where(c => c.CountryId == countryId).OrderBy(c => c.CountryId);
            if (regionId != -1)
                filteredGeography = filteredGeography.Where(r => r.RegionId == regionId).OrderBy(c => c.RegionId);
            if (cityId != -1)
                filteredGeography = filteredGeography.Where(c => c.CityId == cityId).OrderBy(c => c.CityId);

            var cities = filteredGeography.Select(c => c.CityId);

            var breweries = from c in _context.Breweries
                            where cities.Contains(c.CityId)
                            select c;

            return breweries.Include(x => x.City.Region.Country.Continent);
        }
    }
}
