using System;
using System.Collections.Generic;
using System.ServiceModel;
using CapsCollection.Business.BuisenessServices.Interfaces;
using CapsCollection.Business.DTOs;
using CapsCollection.Web.ServiceHost.Contracts;
using CapsCollection.Web.ServiceHost.ServiceBehaviors;

namespace CapsCollection.Web.ServiceHost.Implementations
{
    [UnityInstanceProviderServiceBehavior()]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class BeerService : IBeerService
    {
        #region Members

        private readonly IBreweryBuisenessService _breweryBuisenessService;
        private readonly ICollectionBuisenessService _collectionBuisenessService;
        private readonly IBeerStyleBuisenessService _beerStyleBuisenessService;

        #endregion


        #region Constructor

        public BeerService(IBreweryBuisenessService breweryBuisenessService, ICollectionBuisenessService collectionBuisenessService, IBeerStyleBuisenessService beerStyleBuisenessService)
        {
            if (breweryBuisenessService == null)
                throw new ArgumentNullException("breweryBuisenessService");
            if (collectionBuisenessService == null)
                throw new ArgumentNullException("collectionBuisenessService");
            if (beerStyleBuisenessService == null)
                throw new ArgumentNullException("beerStyleBuisenessService");

            _breweryBuisenessService = breweryBuisenessService;
            _collectionBuisenessService = collectionBuisenessService;
            _beerStyleBuisenessService = beerStyleBuisenessService;
        }


        #endregion


        #region Implementation

        public void ProcessImageFiles(IList<ImageFileOperationDto> imageFiles)
        {
            _collectionBuisenessService.ProcessImageFiles(imageFiles);
        }

        public void DeleteAllFilesFromContainer(string imageContainer)
        {
            _collectionBuisenessService.DeleteAllFilesFromContainer(imageContainer);
        }

        public byte[] DownloadImageFile(string container, string fileName)
        {
            return _collectionBuisenessService.DownloadImageFile(container, fileName);
        }
        public List<BreweryDto> GetBreweries()
        {
            return _breweryBuisenessService.GetBreweries();
        }

        public List<BreweryDto> GetBreweriesByCity(int cityId)
        {
            return _breweryBuisenessService.GetBreweriesByCity(cityId);
        }

        public List<BreweryDto> GetBreweriesByFilter(int continentId, int countryId, int regionId, int cityId)
        {
            return _breweryBuisenessService.GetBreweriesByFilter(continentId, countryId, regionId, cityId);
        }

        public BreweryDto GetBrewery(int breweryId)
        {
            return _breweryBuisenessService.GetBrewery(breweryId);
        }

        public void UpdateBrewery(BreweryDto brewery)
        {
            _breweryBuisenessService.SaveBrewery(brewery);
        }

        public void DeleteBrewery(BreweryDto brewery)
        {
            _breweryBuisenessService.DeleteBrewery(brewery);
        }

        public List<BeerDto> GetAllBeers()
        {
            return _collectionBuisenessService.GetAllBeers();
        }

        public List<BeerDto> GetCountryBeers(int countryId)
        {
            return _collectionBuisenessService.GetCountryBeers(countryId);
        }
        public List<BeerDto> GetBeerByName(string beerName)
        {
            return _collectionBuisenessService.GetBeerByName(beerName);
        }
        public BeerDto GetBeer(int beerId)
        {
            return _collectionBuisenessService.GetBeer(beerId);
        }

        public int UpdateBeer(BeerDto beer)
        {
            return _collectionBuisenessService.SaveBeer(beer);
        }

        public void DeleteBeer(BeerDto beer)
        {
            _collectionBuisenessService.DeleteBeer(beer);
        }

        public List<BreweryDto> GetBreweriesByCountry(int countryId)
        {
            return _collectionBuisenessService.GetBreweriesByCountry(countryId);
        }

        public List<BeerStyleDto> GetBeerStyles()
        {
            return _beerStyleBuisenessService.GetBeerStyles();
        }

        public BeerStyleDto GetBeerStyle(int beerStyleId)
        {
            return _beerStyleBuisenessService.GetBeerStyle(beerStyleId);
        }

        public void UpdateBeerStyle(BeerStyleDto beerStyle)
        {
            _beerStyleBuisenessService.SaveBeerStyle(beerStyle);
        }

        public void DeleteBeerStyle(BeerStyleDto beerStyle)
        {
            _beerStyleBuisenessService.DeleteBeerStyle(beerStyle);
        }

        public List<CapTypeDto> GetCapTypes()
        {
            return _collectionBuisenessService.GetCapTypes();
        }

        #endregion

    }
}
