namespace CapsCollection.Data.Models
{
    public class Geography_City
    {
        public int CityID { get; set; }
        public int RegionID { get; set; }
        public string EnglishCityName { get; set; }
        public string NationalCityName { get; set; }
        
        public virtual Geography_Region Region { get; set; }
    }
}
