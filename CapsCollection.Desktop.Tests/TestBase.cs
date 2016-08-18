using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CapsCollection.Business.DTOs;
using CapsCollection.Common.Settings;
using CapsCollection.Desktop.Infrastructure.Models;
using CapsCollection.Desktop.UI.Modules.Home.ViewModels;
using CapsCollection.Desktop.UI.Modules.Services;

namespace CapsCollection.Desktop.Tests
{
    public class TestBase
    {
        protected string _bottlesPath;
        protected string _capsPath;
        protected string _labelsPath;
        protected string _fakePath;
        protected int _bottlesFilesCount;
        protected int _capsFilesCount;
        protected int _labelsFilesCount;
        protected Dictionary<int, List<ImageData>> _combinedImages;

        protected List<BeerStyleDto> _beerStyleList;
        protected List<CountryDto> _countryList;
        protected List<BreweryDto> _breweryList;
        protected List<CapTypeDto> _capTypeList;
        protected List<BeerDto> _beerMatchList;
        protected List<BeerDto> _beerNotMatchList;
        protected BeerAggregationData _beerAggregationDataWithMatch;
        protected BeerAggregationData _beerAggregationDataWithNoMatch;

        protected List<ImageDataWithThumbnails> _imageDataWithThumbnails;
        protected List<ImageDataWithThumbnails> _imageDataWithThumbnailsOther;
        protected List<ImageDataWithThumbnails> _imageDataWithThumbnailsAnother;

        protected BeerImage _bottleImage;
        protected BeerImage _capImage;
        protected BeerImage _labelImage;

        protected BeerDto Existing;

        public TestBase()
        {
            var rootDirectoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            _bottlesPath = Path.Combine(rootDirectoryPath, @"TestData\Bottles");
            _capsPath = Path.Combine(rootDirectoryPath, @"TestData\Caps");
            _labelsPath = Path.Combine(rootDirectoryPath, @"TestData\Labels");
            _fakePath = Path.Combine(rootDirectoryPath, "fake_folder");

            var allowedExtensions = CapsCollectionSettings.AllowedImageExtensions;

            _bottlesFilesCount = Directory.GetFiles(_bottlesPath).Count(x => allowedExtensions.Any(x.ToLower().EndsWith));
            _capsFilesCount = Directory.GetFiles(_capsPath).Count(x => allowedExtensions.Any(x.ToLower().EndsWith));
            _labelsFilesCount = Directory.GetFiles(_labelsPath).Count(x => allowedExtensions.Any(x.ToLower().EndsWith));

            PrepareCombinedImages();
            PrepareBeerData();
        }

        private void PrepareCombinedImages()
        {
            var imageTypeAggragator = new ImageTypeAggregator();
            imageTypeAggragator.PutImageType(_bottlesPath, ImageType.Bottle);
            imageTypeAggragator.PutImageType(_capsPath, ImageType.Cap);
            imageTypeAggragator.PutImageType(_labelsPath, ImageType.Label);
            _combinedImages = imageTypeAggragator.CombineImages();
        }

        private void PrepareBeerData()
        {
            _beerStyleList = new List<BeerStyleDto>()
            {
                new BeerStyleDto() { BeerStyleId = 1, BeerStyleName = "BeerStyleName 1"},
                new BeerStyleDto() { BeerStyleId = 2, BeerStyleName = "BeerStyleName 2"},
                new BeerStyleDto() { BeerStyleId = 3, BeerStyleName = "BeerStyleName 3"}
            };

            _countryList = new List<CountryDto>()
            {
                new CountryDto() { CountryId = 1, EnglishCountryName = "EnglishCountryName 1", ContinentId = 1},
                new CountryDto() { CountryId = 2, EnglishCountryName = "EnglishCountryName 2"},
                new CountryDto() { CountryId = 3, EnglishCountryName = "EnglishCountryName 3"}
            };

            _capTypeList = new List<CapTypeDto>()
            {
                new CapTypeDto() { CapTypeId = 1, CapTypeName = "CapTypeName 1"},
                new CapTypeDto() { CapTypeId = 2, CapTypeName = "CapTypeName 2"},
                new CapTypeDto() { CapTypeId = 3, CapTypeName = "CapTypeName 3"}
            };

            _breweryList = new List<BreweryDto>()
            {
                new BreweryDto() { BreweryId = 1, Brewery = "Brewery 1", CountryId = 1},
                new BreweryDto() { BreweryId = 2, Brewery = "Brewery 2", CountryId = 1},
                new BreweryDto() { BreweryId = 3, Brewery = "Brewery 3", CountryId = 2}
            };

            _beerMatchList = new List<BeerDto>()
            {
                new BeerDto() { BeerId = 1, BeerName = "BeerName 1"},
                new BeerDto() { BeerId = 2, BeerName = "BeerName 2"},
                new BeerDto() { BeerId = 3, BeerName = "BeerName 3"}
            };

            _beerNotMatchList = new List<BeerDto>()
            {
                new BeerDto() { BeerId = -1, BeerName = "BeerName 1"},
                new BeerDto() { BeerId = -2, BeerName = "BeerName 2"},
                new BeerDto() { BeerId = -3, BeerName = "BeerName 3"}
            };

            _beerAggregationDataWithMatch = new BeerAggregationData()
            {
                BeerStyles = _beerStyleList,
                Breweries = _breweryList,
                CapTypes = _capTypeList,
                Countries = _countryList,
                ExistingBeers = _beerMatchList
            };

            _beerAggregationDataWithNoMatch = new BeerAggregationData()
            {
                BeerStyles = _beerStyleList,
                Breweries = _breweryList,
                CapTypes = _capTypeList,
                Countries = _countryList,
                ExistingBeers = _beerNotMatchList
            };

            Existing = new BeerDto()
            {
                BeerName = "Existing beer name",
                BeerType = "Existing beer type",
                BeerComment = "Existing beer comment",
                BreweryId = 1,
                CapTypeId = 1,
                BeerStyleId = 1,
                CountryId = 1,
                BeerSite = "Existing beer site",
                BeerId = 1,
                BeerPrice = 50,
                BeerYear = new DateTime(2015, 1, 1),
                ContinentId = 1,
                DateAdded = DateTime.Now,
                EnglishBeerName = "Existing english beer name",
                CountryAlpha3 = "Alpha3"
            };
        }

        protected void PrepareThumbnails()
        {
            var imageProcessingService = new ImageProcessingService(new ThumbnailService());
            _imageDataWithThumbnails = imageProcessingService.PrepareThumbnails(_combinedImages.ToList()[0]).Result;
            _imageDataWithThumbnailsOther = imageProcessingService.PrepareThumbnails(_combinedImages.ToList()[1]).Result;
            _imageDataWithThumbnailsAnother = imageProcessingService.PrepareThumbnails(_combinedImages.ToList()[2]).Result;

            var bottleImage = _imageDataWithThumbnails.FirstOrDefault(x => x.ImageType == ImageType.Bottle);
            var capImage = _imageDataWithThumbnails.FirstOrDefault(x => x.ImageType == ImageType.Cap);
            var labelImage = _imageDataWithThumbnails.FirstOrDefault(x => x.ImageType == ImageType.Label);

            _bottleImage = BeerImageBuilder.CreateBuilder(ImageType.Bottle).AttachThumbnails(bottleImage);
            _capImage = BeerImageBuilder.CreateBuilder(ImageType.Cap).AttachThumbnails(capImage);
            _labelImage = BeerImageBuilder.CreateBuilder(ImageType.Label).AttachThumbnails(labelImage);
        }
    }
}