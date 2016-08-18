using System.Runtime.Serialization;

namespace CapsCollection.Business.DTOs
{
    [DataContract]
    public class BreweryDto
    {
        [DataMember]
        public int BreweryId { get; set; }
        [DataMember]
        public int ContinentId { get; set; }
        [DataMember]
        public int CountryId { get; set; }
        [DataMember]
        public int RegionId { get; set; }
        [DataMember]
        public int CityId { get; set; }
        [DataMember]
        public string Brewery { get; set; }
        [DataMember]
        public string Site { get; set; }
        [DataMember]
        public string Comment { get; set; }
    }
}
