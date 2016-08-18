using CapsCollection.Silverlight.ServiceAgents.Proxies.Geography;

namespace CapsCollection.Silverlight.UI.Modules.Brewery.Helpers
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
            {
                return new CountryDto { CountryId = EMPTY_ID, EnglishCountryName = "Select a continent first" };
            }

            return new CountryDto { CountryId = EMPTY_ID, EnglishCountryName = "Select a country" };
        }

        public static bool IsCountryEmpty(CountryDto country)
        {
            return (country == null || EMPTY_ID == country.CountryId);
        }

        public static RegionDto GetEmptyRegion()
        {
            return GetEmptyRegion(null);
        }

        public static RegionDto GetEmptyRegion(CountryDto country)
        {
            if (IsCountryEmpty(country))
            {
                return new RegionDto { RegionId = EMPTY_ID, EnglishRegionName = "Select a country first" };
            }

            return new RegionDto { RegionId = EMPTY_ID, EnglishRegionName = "Select a region" };
        }

        public static bool IsRegionEmpty(RegionDto region)
        {
            return (region == null || EMPTY_ID == region.RegionId);
        }

        public static CityDto GetEmptyCity()
        {
            return GetEmptyCity(null);
        }

        public static CityDto GetEmptyCity(RegionDto region)
        {
            if (IsRegionEmpty(region))
            {
                return new CityDto { CityId = EMPTY_ID, EnglishCityName = "Select a region first" };
            }

            return new CityDto { CityId = EMPTY_ID, EnglishCityName = "Select a city" };
        }

        public static bool IsCityEmpty(CityDto city)
        {
            return (city == null || EMPTY_ID == city.CityId);
        }
    }
}
