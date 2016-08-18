using System.Collections.Generic;
using CapsCollection.Business.DTOs;

namespace CapsCollection.Desktop.Infrastructure.Models
{
    public class BeerAggregationData
    {
        public BeerAggregationData()
        {
            BeerStyles = new List<BeerStyleDto>();
            CapTypes = new List<CapTypeDto>();
            Countries = new List<CountryDto>();
            Breweries = new List<BreweryDto>();
            ExistingBeers = new List<BeerDto>();
        }

        public List<BeerStyleDto> BeerStyles { get; set; }
        public List<CapTypeDto> CapTypes { get; set; }
        public List<CountryDto> Countries { get; set; }
        public List<BreweryDto> Breweries { get; set; }
        public List<BeerDto> ExistingBeers { get; set; }

        public bool AllDataCollected()
        {
            if (BeerStyles.Count == 0 || CapTypes.Count == 0 || Countries.Count == 0 || Breweries.Count == 0 || ExistingBeers.Count == 0)
            {
                return false;
            }
            return true;
        }
    }
}