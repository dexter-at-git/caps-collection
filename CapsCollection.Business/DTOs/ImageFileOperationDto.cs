using System.Runtime.Serialization;

namespace CapsCollection.Business.DTOs
{
    [DataContract]
    public class ImageFileOperationDto
    {
        [DataMember]
        public byte[] ImageBytes { get; set; }
        [DataMember]
        public string ParentContainer { get; set; }
        [DataMember]
        public string Container { get; set; }
        [DataMember]
        public string FileName { get; set; }
        [DataMember]
        public string FileNameTemplate { get; set; }
        [DataMember]
        public string OldFileName { get; set; }
        [DataMember]
        public FileOperation FileOperation { get; set; }
        [DataMember]
        public string SourceFileFullName { get; set; }
    }
}
