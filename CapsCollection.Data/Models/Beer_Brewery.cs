namespace CapsCollection.Data.Models
{
    public class Beer_Brewery
    {
        public int BreweryId { get; set; }
        public int CityId { get; set; }
        public string Brewery { get; set; }
        public string Site { get; set; }
        public string Comment { get; set; }

        public virtual Geography_City City { get; set; }
    }
}
