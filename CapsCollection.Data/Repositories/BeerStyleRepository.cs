using CapsCollection.Data.DataContext;
using CapsCollection.Data.Models;
using CapsCollection.Data.Repositories.Interfaces;

namespace CapsCollection.Data.Repositories
{
    public class BeerStyleRepository : GenericRepository<Beer_BeerStyle>, IBeerStyleRepository
    {
        private readonly CapsCollectionContext _context;

        public BeerStyleRepository(CapsCollectionContext context) : base(context)
        {
            _context = context;
        }
    }
}
