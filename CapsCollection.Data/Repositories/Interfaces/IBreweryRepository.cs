using System.Collections.Generic;
using CapsCollection.Data.Models;

namespace CapsCollection.Data.Repositories.Interfaces
{
    public interface IBreweryRepository : IGenericRepository<Beer_Brewery>
    {
        IEnumerable<Beer_Brewery> GetBreweriesByCity(int cityId);
        IEnumerable<Beer_Brewery> GetBreweriesByFilter(int continentId, int countryId, int regionId, int cityId);
        IEnumerable<Beer_Brewery> GetAllBreweries();
    }
}
