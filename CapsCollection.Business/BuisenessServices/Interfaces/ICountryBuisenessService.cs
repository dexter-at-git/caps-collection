using System.Collections.Generic;
using CapsCollection.Business.DTOs;

namespace CapsCollection.Business.BuisenessServices.Interfaces
{
    public interface ICountryBuisenessService 
    {
        List<ContinentDto> GetContinents();
        List<CountryDto> GetAllCountries();
        List<CountryDto> GetCountriesByContinent(int continentId, bool includeFlags);
        CountryDto GetCountry(int countryId);
        int SaveCountry(CountryDto country, IEnumerable<ImageFileOperationDto> flags);
        void DeleteCountry(CountryDto country);
    }
}
