using CapsCollection.Silverlight.ServiceAgents.Proxies.Beer;
using CapsCollection.Silverlight.ServiceAgents.Proxies.Geography;

namespace CapsCollection.Silverlight.UI.Modules.Collection.Helpers
{
    public class EmptyValues
    {
        private const int EMPTY_ID = -1;

        public static ContinentDto GetEmptyContinent()
        {
            return new ContinentDto { ContinentId = EMPTY_ID, EnglishContinentName = "Select a continent" };
        }

        public static bool IsContinentEmpty(ContinentDto continent)
        {
            return (continent == null || EMPTY_ID == continent.ContinentId);
        }

        public static CountryDto GetEmptyCountry()
        {
            return GetEmptyCountry(null);
        }

        public static CountryDto GetEmptyCountry(ContinentDto continent)
        {
            if (IsContinentEmpty(continent))
                return new CountryDto { CountryId = EMPTY_ID, EnglishCountryName = "Select a continent first" };

            return new CountryDto { CountryId = EMPTY_ID, EnglishCountryName = "Select a country" };
        }

        public static bool IsCountryEmpty(CountryDto country)
        {
            return (country == null || EMPTY_ID == country.CountryId);
        }

        public static BreweryDto GetEmptyBrewery()
        {
            return GetEmptyBrewery(null);
        }

        public static BreweryDto GetEmptyBrewery(CountryDto country)
        {
            if (IsCountryEmpty(country))
                return new BreweryDto { BreweryId = EMPTY_ID, Brewery = "Select a country first" };

            return new BreweryDto { BreweryId = EMPTY_ID, Brewery = "Select a brewery" };
        }

        public static bool IsBreweryEmpty(BreweryDto brewery)
        {
            return (brewery == null || EMPTY_ID == brewery.BreweryId);
        }

        public static BeerStyleDto GetEmptyBeerStyle()
        {
            return new BeerStyleDto { BeerStyleId = EMPTY_ID, BeerStyleName = "Select a beer style" };
        }

        public static CapTypeDto GetEmptyCapType()
        {
            return new CapTypeDto { CapTypeId = EMPTY_ID, CapTypeName = "Select a cap type" };
        }
    }
}
