using System;

namespace CapsCollection.Data.Models
{
    public class Beer_Beer
    {
        public int BeerId { get; set; }
        public int CountryId { get; set; }
        public string BeerName { get; set; }
        public string BeerType { get; set; }
        public int BeerStyleID { get; set; }
        public decimal? BeerPrice { get; set; }
        public DateTime? BeerYear { get; set; }
        public DateTime DateAdded { get; set; }
        public string BeerSite { get; set; }
        public string BeerComment { get; set; }
        public int BreweryId { get; set; }
        public int CapTypeID { get; set; }

        public virtual Geography_Country Country { get; set; }
    }
}
