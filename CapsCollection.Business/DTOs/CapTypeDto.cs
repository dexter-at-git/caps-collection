using System.Runtime.Serialization;

namespace CapsCollection.Business.DTOs
{
    [DataContract]
    public class CapTypeDto
    {
        [DataMember]
        public int CapTypeId { get; set; }
        [DataMember]
        public string CapTypeName { get; set; }
    }
}
