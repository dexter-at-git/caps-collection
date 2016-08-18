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
    public class CollectionBuisenessService : ICollectionBuisenessService
    {
        private readonly ICollectionRepository _collectionRepository;
        private readonly IImageFileRepository _imageFileRepository;
        private readonly IBeerCapTypeRepository _beerCapTypeRepository;


        public CollectionBuisenessService(ICollectionRepository collectionRepository, IImageFileRepository imageFileRepository, IBeerCapTypeRepository beerCapTypeRepository)
        {
            if (collectionRepository == null)
                throw new ArgumentNullException("collectionRepository");
            if (imageFileRepository == null)
                throw new ArgumentNullException("imageFileRepository");

            _collectionRepository = collectionRepository;
            _imageFileRepository = imageFileRepository;
            _beerCapTypeRepository = beerCapTypeRepository;
        }


        public void ProcessImageFiles(IList<ImageFileOperationDto> imageFiles)
        {
            foreach (var imageFile in imageFiles)
            {
                if (imageFile.FileOperation == FileOperation.Save)
                {
                    _imageFileRepository.UploadImageFile(imageFile.ImageBytes, imageFile.ParentContainer, imageFile.Container, imageFile.FileName);
                }

                if (imageFile.FileOperation == FileOperation.Delete)
                {
                    _imageFileRepository.DeleteImageFile(imageFile.ParentContainer, imageFile.Container, imageFile.FileName);
                }
            }
        }

        public byte[] DownloadImageFile(string container, string fileName)
        {
            return _imageFileRepository.DownloadImageFile(container, fileName);
        }
        
        public void DeleteAllFilesFromContainer(string imageContainer)
        {
            _imageFileRepository.DeleteAllFiles(imageContainer);
        }

        public List<BeerCountryDto> GetBeerCountries()
        {
            var countries = _collectionRepository.GetBeerCountries().ToList();
            var beers = _collectionRepository.GetAllBeers().ToList();

            var countriesDto = Mapper.Map<List<BeerCountryDto>>(countries);

            countriesDto.ForEach(country =>
            {
                country.BeerCount = beers.Count(x => x.CountryId == country.CountryId);
            });

            return countriesDto.OrderBy(x => x.EnglishCountryName).ToList();
        }
        
        public List<BeerDto> GetCountryBeers(int countryId)
        {
            var beers = _collectionRepository.GetCountryBeers(countryId).ToList();
            var beersDto = Mapper.Map<List<BeerDto>>(beers);

            return beersDto.OrderBy(b => b.BeerName).ThenBy(b => b.BeerType).ToList();
        }
        
        public List<BeerDto> GetAllBeers()
        {
            var beers = _collectionRepository.GetAllBeers().ToList();
            var beersDto = Mapper.Map<List<BeerDto>>(beers);

            return beersDto.OrderBy(x => x.BeerName).ThenBy(b => b.BeerType).ToList();
        }
        
        public BeerDto GetBeer(int beerId)
        {
            var beer = _collectionRepository.GetBeer(beerId);
            var beersDto = Mapper.Map<BeerDto>(beer);

            return beersDto;
        }
        
        public int SaveBeer(BeerDto beerDto)
        {
            if (beerDto == null)
                throw new ArgumentNullException("beerDto");

            var beer = Mapper.Map<Beer_Beer>(beerDto);

            if (beerDto.BeerId != 0)
            {
                _collectionRepository.Update(beer);
            }
            else
            {
                beer.DateAdded = DateTime.Now;
                _collectionRepository.Insert(beer);
            }

            _collectionRepository.Save();

            return beer.BeerId;
        }
        
        public void DeleteBeer(BeerDto beerDto)
        {
            if (beerDto == null)
                throw new ArgumentNullException("beerDto");

            var beer = Mapper.Map<Beer_Beer>(beerDto);

            _collectionRepository.Remove(beer);
            _collectionRepository.Save();
        }
        
        public List<BreweryDto> GetBreweriesByCountry(int countryId)
        {
            var breweries = _collectionRepository.GetBreweriesByCountry(countryId).ToList();
            var breweriesDto = Mapper.Map<List<BreweryDto>>(breweries);

            return breweriesDto.OrderBy(x => x.Brewery).ToList();
        }
        
        public List<ContinentDto> GetContinentsWithBreweries()
        {
            var continents = _collectionRepository.GetContinentsWithBreweries().ToList();
            var continentsDto = Mapper.Map<List<ContinentDto>>(continents);

            return continentsDto.OrderBy(x => x.EnglishContinentName).ToList();
        }
        
        public List<CountryDto> GetContinentCountriesWithBreweries(int continentId)
        {
            var countries = _collectionRepository.GetContinentCountriesWithBreweries(continentId).ToList();
            var countriesDto = Mapper.Map<List<CountryDto>>(countries);

            return countriesDto.OrderBy(x => x.EnglishCountryName).ToList();
        }
        
        public List<CountryDto> GetCountriesWithBreweries()
        {
            var countries = _collectionRepository.GetCountriesWithBreweries().ToList();
            var countriesDto = Mapper.Map<List<CountryDto>>(countries);

            return countriesDto.OrderBy(x => x.EnglishCountryName).ToList();
        }
        
        public List<CapTypeDto> GetCapTypes()
        {
            var capTypes = _beerCapTypeRepository.GetAll().ToList();
            var capTypesDto = Mapper.Map<List<CapTypeDto>>(capTypes);

            return capTypesDto.ToList();
        }
        public List<BeerDto> GetBeerByName(string beerName)
        {
            var beers = _collectionRepository.GetBeerByName(beerName).ToList();
            var beersDto = Mapper.Map<List<BeerDto>>(beers);

            return beersDto.OrderBy(b => b.BeerName).ThenBy(b => b.BeerType).ToList();
        }
    }
}
