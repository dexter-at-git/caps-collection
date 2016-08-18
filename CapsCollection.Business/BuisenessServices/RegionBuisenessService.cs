using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CapsCollection.Business.BuisenessServices.Interfaces;
using CapsCollection.Business.DTOs;
using CapsCollection.Data.Models;
using CapsCollection.Data.Repositories.Interfaces;

namespace CapsCollection.Business.BuisenessServices
{
    public class RegionBuisenessService : IRegionBuisenessService
    {
        readonly IRegionRepository _regionRepository;

        public RegionBuisenessService(IRegionRepository regionRepository)
        {
            if (regionRepository == null)
                throw new ArgumentNullException("regionRepository");

            _regionRepository = regionRepository;
        }


        public List<RegionDto> GetAllRegions()
        {
            var regions = _regionRepository.GetAll().ToList();
            var regionsDto = Mapper.Map<List<RegionDto>>(regions);

            return regionsDto.OrderBy(x => x.EnglishRegionName).ToList();
        }
        
        public List<RegionDto> GetRegionsByCountry(int countryId)
        {
            var regions = _regionRepository.GetRegionsByCountry(countryId);
            var regionsDto = Mapper.Map<List<RegionDto>>(regions);

            return regionsDto.OrderBy(x => x.EnglishRegionName).ToList();
        }
        
        public RegionDto GetRegion(int regionId)
        {
            var region = _regionRepository.Get(regionId);
            var regionDto = Mapper.Map<RegionDto>(region);

            return regionDto;
        }
        
        public int SaveRegion(RegionDto regionDto)
        {
            if (regionDto == null)
                throw new ArgumentNullException("regionDto");

            var region = Mapper.Map<Geography_Region>(regionDto);

            if (regionDto.RegionId != 0)
            {
                _regionRepository.Update(region);
            }
            else
            {
                _regionRepository.Insert(region);
            }

            _regionRepository.Save();

            return region.RegionID;
        }

        public void DeleteRegion(RegionDto regionDto)
        {
            if (regionDto == null)
                throw new ArgumentNullException("regionDto");

            var region = Mapper.Map<Geography_Region>(regionDto);

            _regionRepository.Remove(region);
            _regionRepository.Save();
        }
    }
}
