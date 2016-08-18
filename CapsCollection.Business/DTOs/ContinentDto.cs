using System.Runtime.Serialization;

namespace CapsCollection.Business.DTOs
{
    [DataContract]
    public class ContinentDto
    {
        [DataMember]
        public int ContinentId { get; set; }
        [DataMember]
        public string ContinentName { get; set; }
        [DataMember]
        public string EnglishContinentName { get; set; }
        [DataMember]
        public string ContinentCode { get; set; }
    }
}
