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
    public class CityBuisenessService : ICityBuisenessService
    {
        readonly ICityRepository _cityRepository;

        public CityBuisenessService(ICityRepository cityRepository)
        {
            if (cityRepository == null)
                throw new ArgumentNullException("cityRepository");

            _cityRepository = cityRepository;
        }


        public List<CityDto> GetAllCities()
        {
            var cities = _cityRepository.GetAll().ToList();
            var citiesDto = Mapper.Map<List<CityDto>>(cities);

            return citiesDto.OrderBy(x => x.EnglishCityName).ToList();
        }

        public List<CityDto> GetCitiesByRegion(int regionId)
        {
            var cities = _cityRepository.GetCitiesByRegion(regionId).ToList();
            var citiesDto = Mapper.Map<List<CityDto>>(cities);

            return citiesDto.OrderBy(x => x.EnglishCityName).ToList();
        }

        public CityDto GetCity(int cityId)
        {
            var city = _cityRepository.Get(cityId);
            var cityDto = Mapper.Map<CityDto>(city);

            return cityDto;
        }

        public int SaveCity(CityDto cityDto)
        {
            if (cityDto == null)
                throw new ArgumentNullException("cityDto");

            var city = Mapper.Map<Geography_City>(cityDto);

            if (cityDto.CityId != 0)
            {
                _cityRepository.Update(city);
            }
            else
            {
                _cityRepository.Insert(city);
            }

            _cityRepository.Save();

            return city.CityID;
        }

        public void DeleteCity(CityDto cityDto)
        {
            if (cityDto == null)
                throw new ArgumentNullException("cityDto");

            var city = Mapper.Map<Geography_City>(cityDto);

            _cityRepository.Remove(city);
            _cityRepository.Save();
        }
    }
}
