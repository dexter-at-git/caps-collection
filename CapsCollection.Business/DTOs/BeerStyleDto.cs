using System.Runtime.Serialization;

namespace CapsCollection.Business.DTOs
{
    [DataContract]
    public class BeerStyleDto
    {
        [DataMember]
        public int BeerStyleId { get; set; }
        [DataMember]
        public string BeerStyleName { get; set; }
    }
}
