using System.Collections.Generic;
using CapsCollection.Data.Models;

namespace CapsCollection.Data.Repositories.Interfaces
{
    public interface ICountryRepository : IGenericRepository<Geography_Country>
    {
        IEnumerable<Geography_Country> GetCountriesByContinent(int continentId);
    }
}
