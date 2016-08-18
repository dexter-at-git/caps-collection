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
    public class CountryBuisenessService : ICountryBuisenessService
    {
        private readonly IContinentRepository _continentRepository;
        private readonly IImageFileRepository _imageFileRepository;
        private readonly ICountryRepository _countryRepository;


        public CountryBuisenessService(ICountryRepository countryRepository, IContinentRepository continentRepository, IImageFileRepository imageFileRepository)
        {
            if (countryRepository == null)
                throw new ArgumentNullException("countryRepository");
            if (continentRepository == null)
                throw new ArgumentNullException("continentRepository");
            if (imageFileRepository == null)
                throw new ArgumentNullException("imageFileRepository");

            _countryRepository = countryRepository;
            _continentRepository = continentRepository;
            _imageFileRepository = imageFileRepository;
        }


        public List<ContinentDto> GetContinents()
        {
            var continents = _continentRepository.GetAll().ToList();
            var continentsDto = Mapper.Map<List<ContinentDto>>(continents);

            return continentsDto.OrderBy(x => x.EnglishContinentName).ToList();
        }

        public List<CountryDto> GetAllCountries()
        {
            var countries = _countryRepository.GetAll();
            var countriesDto = Mapper.Map<List<CountryDto>>(countries);

            return countriesDto.OrderBy(x => x.EnglishCountryName).ToList();
        }

        public List<CountryDto> GetCountriesByContinent(int continentId, bool includeFlags)
        {
            var countries = _countryRepository.GetCountriesByContinent(continentId);
            var countriesDto = Mapper.Map<List<CountryDto>>(countries);

            return countriesDto.OrderBy(x => x.EnglishCountryName).ToList();
        }

        public CountryDto GetCountry(int countryId)
        {
            var country = _countryRepository.Get(countryId);
            var countryDto = Mapper.Map<CountryDto>(country);

            return countryDto;
        }

        public int SaveCountry(CountryDto countryDto, IEnumerable<ImageFileOperationDto> flags)
        {
            if (countryDto == null)
                throw new ArgumentNullException("countryDto");

            var country = Mapper.Map<Geography_Country>(countryDto);

            if (countryDto.CountryId != 0)
            {
                _countryRepository.Update(country);
            }
            else
            {
                _countryRepository.Insert(country);
            }

            _countryRepository.Save();

            ProcessImageFiles(flags.ToList());

            return country.CountryID;
        }
        
        private void ProcessImageFiles(IList<ImageFileOperationDto> imageFiles)
        {
            foreach (var imageFile in imageFiles)
            {
                if (imageFile.FileOperation == FileOperation.Save)
                {
                    _imageFileRepository.UploadImageFile(imageFile.ImageBytes, imageFile.ParentContainer, imageFile.Container, imageFile.FileName);
                }

                if (imageFile.FileOperation == FileOperation.Update)
                {
                    _imageFileRepository.UpdateImageFile(imageFile.Container, imageFile.FileName, imageFile.OldFileName);
                }

                if (imageFile.FileOperation == FileOperation.Delete)
                {
                    _imageFileRepository.DeleteImageFile(imageFile.ParentContainer, imageFile.Container, imageFile.FileName);
                }
            }
        }
        
        public void DeleteCountry(CountryDto countryDto)
        {
            if (countryDto == null)
                throw new ArgumentNullException("countryDto");

            var country = Mapper.Map<Geography_Country>(countryDto);

            _countryRepository.Remove(country);
            _countryRepository.Save();
        }
    }
}
