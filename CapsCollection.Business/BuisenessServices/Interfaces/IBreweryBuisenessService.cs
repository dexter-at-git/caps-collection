using System.Collections.Generic;
using CapsCollection.Business.DTOs;

namespace CapsCollection.Business.BuisenessServices.Interfaces
{
    public interface IBreweryBuisenessService
    {
        List<BreweryDto> GetBreweries();
        List<BreweryDto> GetBreweriesByCity(int cityId);
        List<BreweryDto> GetBreweriesByFilter(int continentId, int countryId, int regionId, int cityId);
        BreweryDto GetBrewery(int breweryId);
        int SaveBrewery(BreweryDto brewery);
        void DeleteBrewery(BreweryDto brewery);
    }
}
