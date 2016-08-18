﻿using System.Runtime.Serialization;

namespace CapsCollection.Business.DTOs
{
    [DataContract]
    public class BeerCountryDto
    {
        [DataMember]
        public int CountryId { get; set; }
        [DataMember]
        public int ContinentId { get; set; }
        [DataMember]
        public ContinentDto Continent { get; set; }
        [DataMember]
        public string EnglishCountryName { get; set; }
        [DataMember]
        public string EnglishCountryFullName { get; set; }
        [DataMember]
        public string NationalCountryName { get; set; }
        [DataMember]
        public string NationalCountryFullName { get; set; }
        [DataMember]
        public string Alpha3 { get; set; }
        [DataMember]
        public int BeerCount { get; set; }
    }
}
