using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using CapsCollection.Data.DataContext;
using CapsCollection.Data.Models;
using CapsCollection.Data.Repositories.Interfaces;

namespace CapsCollection.Data.Repositories
{
    public class RegionRepository : GenericRepository<Geography_Region>, IRegionRepository
    {
        private readonly CapsCollectionContext _context;

        public RegionRepository(CapsCollectionContext context) : base(context)
        {
            _context = context;
        }


        public IEnumerable<Geography_Region> GetRegionsByCountry(int countryId)
        {
            return _context.Regions.Include(x => x.Country.Continent).Where(c => c.CountryID == countryId);
        }
    }
}
