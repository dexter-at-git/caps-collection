using System.Collections.Generic;
using CapsCollection.Data.Models;

namespace CapsCollection.Data.Repositories.Interfaces
{
    public interface IRegionRepository : IGenericRepository<Geography_Region>
    {
        IEnumerable<Geography_Region> GetRegionsByCountry(int countryId);
    }
}
