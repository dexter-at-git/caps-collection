using CapsCollection.Data.DataContext;
using CapsCollection.Data.Models;
using CapsCollection.Data.Repositories.Interfaces;

namespace CapsCollection.Data.Repositories
{
    public class ContinentRepository : GenericRepository<Geography_Continent>, IContinentRepository
    {
        private readonly CapsCollectionContext _context;

        public ContinentRepository(CapsCollectionContext context) : base(context)
        {
            _context = context;
        }
    }
}
