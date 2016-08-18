using System.ServiceModel;
using System.ServiceModel.Channels;
using CapsCollection.Business.DTOs;
using CapsCollection.Common.Settings;
using CapsCollection.Desktop.UI.Modules.Services.WcfService;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CapsCollection.Desktop.Infrastructure.Models;
using CapsCollection.Desktop.UI.Modules.Services.Interfaces;
using CapsCollection.Web.ServiceHost.Contracts;

namespace CapsCollection.Desktop.UI.Modules.Services
{
    public class BeerRepository : IBeerServiceRepository
    {
        private readonly Binding _binding;
        private readonly EndpointAddress _beerServiceEndpointAddress;
        private readonly EndpointAddress _geographyServiceEndpointAddress;

        public BeerRepository()
        {
            _binding = CapsCollectionEndpointSettings.HttpBinding;
            _beerServiceEndpointAddress = CapsCollectionEndpointSettings.BeerServiceEndpointAddress;
            _geographyServiceEndpointAddress = CapsCollectionEndpointSettings.GeographyServiceEndpointAddress;
        }

        public IList<CountryDto> GetCountriesWithBrewery()
        {
            using (var client = new ServiceClientWrapper<IGeographyService>(_binding, _geographyServiceEndpointAddress))
            {
                return client.Client.GetCountriesWithBreweries();
            }
        }

        public IList<BeerDto> GetAllBeers()
        {
            using (var client = new ServiceClientWrapper<IBeerService>(_binding, _beerServiceEndpointAddress))
            {
                return client.Client.GetAllBeers();
            }
        }

        public IList<BeerStyleDto> GetBeerStyles()
        {
            using (var client = new ServiceClientWrapper<IBeerService>(_binding, _beerServiceEndpointAddress))
            {
                return client.Client.GetBeerStyles();
            }
        }

        public IList<CapTypeDto> GetCapTypes()
        {
            using (var client = new ServiceClientWrapper<IBeerService>(_binding, _beerServiceEndpointAddress))
            {
                return client.Client.GetCapTypes();
            }
        }

        public IList<BreweryDto> GetBreweries()
        {
            using (var client = new ServiceClientWrapper<IBeerService>(_binding, _beerServiceEndpointAddress))
            {
                return client.Client.GetBreweries();
            }
        }

        public async Task<int> SaveBeer(BeerDto beer)
        {
            int beerId = 0;
            using (var client = new ServiceClientWrapper<IBeerService>(_binding, _beerServiceEndpointAddress))
            {
                var beerSaveTask = Task.Run(() => beerId = client.Client.UpdateBeer(beer));

                await Task.WhenAll(beerSaveTask);
            }
            return beerId;
        }

        public void EmptyImageContainers()
        {
            /*
            var bottlesContainer = ConfigFileParser.GetAppSetting("BottlesContainer");
            var capsContainer = ConfigFileParser.GetAppSetting("CapsContainer");
            var labelContainer = ConfigFileParser.GetAppSetting("LabelsContainer");
            var flagsContainer = ConfigFileParser.GetAppSetting("FlagsContainer");

            EmptyImageContainer(bottlesContainer);
            EmptyImageContainer(capsContainer);
            EmptyImageContainer(labelContainer);
            EmptyImageContainer(flagsContainer);
            */
        }

        public byte[] DownloadImage(string container, string fileName)
        {
            //  ServiceWrapper<IBeerService> serviceWrapper = null;
            byte[] imageBytes = new byte[0];
            /*
            try
            {
                using (serviceWrapper = new ServiceWrapper<IBeerService>(SERVICE_ENDPOINT_CONFIG_SECTION, MAX_BUFFER_CONFIG_SECTION))
                {
                    imageBytes = serviceWrapper.Channel.DownloadImageFile(container, fileName);
                }
            }
            catch (Exception ex)
            {
                //   throw new Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                if (serviceWrapper != null)
                    serviceWrapper.Dispose();
            }
            */
            return imageBytes;
        }

        public async Task UploadImagesToCloud(List<ImageFileOperationDto> imageFileOperations)
        {
            using (var client = new ServiceClientWrapper<IBeerService>(_binding, _beerServiceEndpointAddress))
            {
                var existingBeersTask = Task.Run(() => client.Client.ProcessImageFiles(imageFileOperations));
                await Task.WhenAll(existingBeersTask);
            }
        }

        private void EmptyImageContainer(string container)
        {
            /*
            ServiceWrapper<IBeerService> serviceWrapper = null;

            try
            {
                using (serviceWrapper = new ServiceWrapper<IBeerService>(SERVICE_ENDPOINT_CONFIG_SECTION, MAX_BUFFER_CONFIG_SECTION))
                {
                    serviceWrapper.Channel.DeleteAllFilesFromContainer(container);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                if (serviceWrapper != null)
                    serviceWrapper.Dispose();
            }
            */
        }

        public async Task<BeerAggregationData> GetBeerAggregationData()
        {
            var beerStyles = new List<BeerStyleDto>();
            var countries = new List<CountryDto>();
            var capTypes = new List<CapTypeDto>();
            var breweries = new List<BreweryDto>();
            var existingBeers = new List<BeerDto>();
            
            var beerStylesTask = Task.Run(() => beerStyles = GetBeerStyles().OrderBy(x => x.BeerStyleName).ToList());
            var countriesTask = Task.Run(() => countries = GetCountriesWithBrewery().OrderBy(x => x.EnglishCountryName).ToList());
            var capTypesTask = Task.Run(() => capTypes = GetCapTypes().ToList());
            var breweriesTask = Task.Run(() => breweries = GetBreweries().OrderBy(x => x.Brewery).ToList());
            var existingBeersTask = Task.Run(() => existingBeers = GetAllBeers().ToList());

            await Task.WhenAll(beerStylesTask, countriesTask, capTypesTask, breweriesTask, existingBeersTask);

            var beerAggregationData = new BeerAggregationData()
            {
                BeerStyles = beerStyles,
                Countries = countries,
                CapTypes = capTypes,
                Breweries = breweries,
                ExistingBeers = existingBeers
            };

            return beerAggregationData;
        }
    }
}
