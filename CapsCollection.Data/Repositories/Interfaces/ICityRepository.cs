using System.Collections.Generic;
using CapsCollection.Data.Models;

namespace CapsCollection.Data.Repositories.Interfaces
{
    public interface ICityRepository : IGenericRepository<Geography_City>
    {
        IEnumerable<Geography_City> GetCitiesByRegion(int regionId);
    }
}
