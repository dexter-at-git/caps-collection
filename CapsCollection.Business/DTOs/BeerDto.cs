using System;
using System.Runtime.Serialization;

namespace CapsCollection.Business.DTOs
{
    [DataContract]
    public class BeerDto
    {
        [DataMember]
        public int BeerId { get; set; }
        [DataMember]
        public int ContinentId { get; set; }
        [DataMember]
        public int CountryId { get; set; }
        [DataMember]
        public string CountryAlpha3 { get; set; }
        [DataMember]
        public string BeerName { get; set; }
        [DataMember]
        public string BeerNameNoDiacritics { get; set; }
        [DataMember]
        public string EnglishBeerName { get; set; }
        [DataMember]
        public string BeerType { get; set; }
        [DataMember]
        public int BeerStyleId { get; set; }
        [DataMember]
        public decimal? BeerPrice { get; set; }
        [DataMember]
        public DateTime? BeerYear { get; set; }
        [DataMember]
        public DateTime DateAdded { get; set; }
        [DataMember]
        public string BeerSite { get; set; }
        [DataMember]
        public string BeerComment { get; set; }
        [DataMember]
        public int BreweryId { get; set; }
        [DataMember]
        public int CapTypeId { get; set; }
    }
}
