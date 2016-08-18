using System.Collections.Generic;
using System.Threading.Tasks;
using CapsCollection.Business.DTOs;
using CapsCollection.Desktop.Infrastructure.Models;

namespace CapsCollection.Desktop.UI.Modules.Services.Interfaces
{
    public interface IBeerServiceRepository
    {
        IList<BeerDto> GetAllBeers();
        IList<CountryDto> GetCountriesWithBrewery();
        IList<BreweryDto> GetBreweries();
        IList<BeerStyleDto> GetBeerStyles();
        IList<CapTypeDto> GetCapTypes();
        Task<int> SaveBeer(BeerDto beer);
        void EmptyImageContainers();
        byte[] DownloadImage(string container, string fileName);
        Task UploadImagesToCloud(List<ImageFileOperationDto> beerImageList);
        Task<BeerAggregationData> GetBeerAggregationData();
    }
}
