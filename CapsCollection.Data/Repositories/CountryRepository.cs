using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using CapsCollection.Data.DataContext;
using CapsCollection.Data.Models;
using CapsCollection.Data.Repositories.Interfaces;

namespace CapsCollection.Data.Repositories
{
    public class CountryRepository : GenericRepository<Geography_Country>, ICountryRepository
    {
        private readonly CapsCollectionContext _context;

        public CountryRepository(CapsCollectionContext context): base(context)
        {
            _context = context;
        }
        

        public IEnumerable<Geography_Country> GetCountriesByContinent(int continentId)
        {
            return _context.Countries.Include(x=>x.Continent).Where(c => c.ContinentID == continentId);
        }
    }
}
