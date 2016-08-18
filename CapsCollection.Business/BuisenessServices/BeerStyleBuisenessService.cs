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
    public class BeerStyleBuisenessService : IBeerStyleBuisenessService
    {
        private readonly IBeerStyleRepository _beerStyleRepository;


        public BeerStyleBuisenessService(IBeerStyleRepository beerStyleRepository)
        {
            if (beerStyleRepository == null)
                throw new ArgumentNullException("beerStyleRepository");

            _beerStyleRepository = beerStyleRepository;
        }


        public List<BeerStyleDto> GetBeerStyles()
        {
            var beerStyles = _beerStyleRepository.GetAll().ToList();
            var beerStylesDto = Mapper.Map<List<BeerStyleDto>>(beerStyles);

            return beerStylesDto.OrderBy(x => x.BeerStyleName).ToList();
        }

        public BeerStyleDto GetBeerStyle(int beerStyleId)
        {
            var beerStyle = _beerStyleRepository.Get(beerStyleId);
            var beerStyleDto = Mapper.Map<BeerStyleDto>(beerStyle);

            return beerStyleDto;
        }

        public int SaveBeerStyle(BeerStyleDto beerStyleDto)
        {
            if (beerStyleDto == null)
                throw new ArgumentNullException("beerStyleDto");

            var beerStyle = Mapper.Map<Beer_BeerStyle>(beerStyleDto);

            if (beerStyleDto.BeerStyleId != 0)
            {
                _beerStyleRepository.Update(beerStyle);
            }
            else
            {
                _beerStyleRepository.Insert(beerStyle);
            }

            _beerStyleRepository.Save();

            return beerStyle.BeerStyleID;
        }

        public void DeleteBeerStyle(BeerStyleDto beerStyleDto)
        {
            if (beerStyleDto == null)
                throw new ArgumentNullException("beerStyleDto");

            var beerStyle = Mapper.Map<Beer_BeerStyle>(beerStyleDto);

            _beerStyleRepository.Remove(beerStyle);
            _beerStyleRepository.Save();
        }
    }
}
