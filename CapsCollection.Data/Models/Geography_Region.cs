namespace CapsCollection.Data.Models
{
    public class Geography_Region
    {
        public int RegionID { get; set; }
        public int CountryID { get; set; }
        public string NationalRegionName { get; set; }
        public string EnglishRegionName { get; set; }
        
        public virtual Geography_Country Country { get; set; }
    }
}
