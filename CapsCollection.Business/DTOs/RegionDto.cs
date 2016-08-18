using System.Runtime.Serialization;

namespace CapsCollection.Business.DTOs
{
    [DataContract]
    public class RegionDto
    {
        [DataMember]
        public int RegionId { get; set; }
        [DataMember]
        public int CountryId { get; set; }
        [DataMember]
        public string EnglishRegionName { get; set; }
        [DataMember]
        public string NationalRegionName { get; set; }
    }
}
