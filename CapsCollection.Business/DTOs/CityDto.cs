using System.Runtime.Serialization;

namespace CapsCollection.Business.DTOs
{
    [DataContract]
    public class CityDto
    {
        [DataMember]
        public int CityId { get; set; }
        [DataMember]
        public int RegionId { get; set; }
        [DataMember]
        public string EnglishCityName { get; set; }
        [DataMember]
        public string NationalCityName { get; set; }
    }
}
