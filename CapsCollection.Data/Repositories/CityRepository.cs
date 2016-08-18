using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using CapsCollection.Data.DataContext;
using CapsCollection.Data.Models;
using CapsCollection.Data.Repositories.Interfaces;

namespace CapsCollection.Data.Repositories
{
    public class CityRepository : GenericRepository<Geography_City>, ICityRepository
    {
        private readonly CapsCollectionContext _context;

        public CityRepository(CapsCollectionContext context) : base(context)
        {
            _context = context;
        }


        public IEnumerable<Geography_City> GetCitiesByRegion(int regionId)
        {
            return _context.Cities.Include(x=>x.Region.Country.Continent).Where(c => c.RegionID == regionId);
        }
    }
}
