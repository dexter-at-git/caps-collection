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
    public class BreweryBuisenessService : IBreweryBuisenessService
    {
        readonly IBreweryRepository _breweryRepository;


        public BreweryBuisenessService(IBreweryRepository breweryRepository)
        {
            if (breweryRepository == null)
                throw new ArgumentNullException("breweryRepository");

            _breweryRepository = breweryRepository;
        }


        public List<BreweryDto> GetBreweries()
        {
            var breweries = _breweryRepository.GetAllBreweries().ToList();
            var breweriesDto = Mapper.Map<List<BreweryDto>>(breweries);

            return breweriesDto.OrderBy(x => x.Brewery).ToList();
        }

        public List<BreweryDto> GetBreweriesByCity(int cityId)
        {
            var breweries = _breweryRepository.GetBreweriesByCity(cityId);
            var breweriesDto = Mapper.Map<List<BreweryDto>>(breweries);

            return breweriesDto.OrderBy(x => x.Brewery).ToList();
        }

        public List<BreweryDto> GetBreweriesByFilter(int continentId, int countryId, int regionId, int cityId)
        {
            var breweries = _breweryRepository.GetBreweriesByFilter(continentId, countryId, regionId, cityId);
            var breweriesDto = Mapper.Map<List<BreweryDto>>(breweries);

            return breweriesDto.OrderBy(x => x.Brewery).ToList();
        }

        public BreweryDto GetBrewery(int breweryId)
        {
            var brewery = _breweryRepository.Get(breweryId);
            var breweryDto = Mapper.Map<BreweryDto>(brewery);

            return breweryDto;
        }

        public int SaveBrewery(BreweryDto breweryDto)
        {
            if (breweryDto == null)
                throw new ArgumentNullException("breweryDto");

            var brewery = Mapper.Map<Beer_Brewery>(breweryDto);

            if (breweryDto.BreweryId != 0)
            {
                _breweryRepository.Update(brewery);
            }
            else
            {
                _breweryRepository.Insert(brewery);
            }

            _breweryRepository.Save();

            return brewery.BreweryId;
        }

        public void DeleteBrewery(BreweryDto breweryDto)
        {
            if (breweryDto == null)
                throw new ArgumentNullException("breweryDto");

            var brewery = Mapper.Map<Beer_Brewery>(breweryDto);

            _breweryRepository.Remove(brewery);
            _breweryRepository.Save();
        }
    }
}
