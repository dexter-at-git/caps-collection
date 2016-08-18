namespace CapsCollection.Silverlight.Infrastructure.Models
{
    public class BreweryFilter
    {
        public int ContinentId;
        public int CountryId;
        public int RegionId;
        public int CityId;

        public BreweryFilter()
        {
            ContinentId = -1;
            CountryId = -1;
            RegionId = -1;
            CityId = -1;
        }
    }
}