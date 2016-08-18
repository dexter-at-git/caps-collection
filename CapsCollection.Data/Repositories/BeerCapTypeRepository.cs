using CapsCollection.Data.DataContext;
using CapsCollection.Data.Models;
using CapsCollection.Data.Repositories.Interfaces;

namespace CapsCollection.Data.Repositories
{
    public class BeerCapTypeRepository : GenericRepository<Beer_CapType>, IBeerCapTypeRepository
    {
        private readonly CapsCollectionContext _context;

        public BeerCapTypeRepository(CapsCollectionContext context) : base(context)
        {
            _context = context;
        }
    }
}