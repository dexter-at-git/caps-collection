using System.Collections.Generic;
using CapsCollection.Business.DTOs;
using CapsCollection.Desktop.Infrastructure.Models;

namespace CapsCollection.Desktop.Infrastructure.Commands
{
    public class BeerLoadDataEventArgs
    {
        public BeerLoadDataEventArgs()
        {
            BeerStyles = new List<BeerStyleDto>();
            CapTypes = new List<CapTypeDto>();
            Countries = new List<CountryDto>();
            Breweries = new List<BreweryDto>();
            // Existing = new BeerDto();

            ImageList = new List<ImageDataWithThumbnails>();
        }

        public List<ImageDataWithThumbnails> ImageList { get; set; }
        public List<BeerStyleDto> BeerStyles { get; set; }
        public List<CapTypeDto> CapTypes { get; set; }
        public List<CountryDto> Countries { get; set; }
        public List<BreweryDto> Breweries { get; set; }
        public BeerDto Existing { get; set; }
    }
}