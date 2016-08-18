using System.Collections.Generic;
using CapsCollection.Business.DTOs;

namespace CapsCollection.Business.BuisenessServices.Interfaces
{
    public interface ICityBuisenessService
    {
        List<CityDto> GetAllCities();
        List<CityDto> GetCitiesByRegion(int regionId);
        CityDto GetCity(int cityId);
        int SaveCity(CityDto city);
        void DeleteCity(CityDto city);
    }
}
