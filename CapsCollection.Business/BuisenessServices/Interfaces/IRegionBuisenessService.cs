using System.Collections.Generic;
using CapsCollection.Business.DTOs;

namespace CapsCollection.Business.BuisenessServices.Interfaces
{
    public interface IRegionBuisenessService
    {
        List<RegionDto> GetAllRegions();
        List<RegionDto> GetRegionsByCountry(int countryId);
        RegionDto GetRegion(int regionId);
        int SaveRegion(RegionDto region);
        void DeleteRegion(RegionDto region);
    }
}
