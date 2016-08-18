using System.IO;

namespace CapsCollection.Desktop.Infrastructure.Models
{
    public class ImageData
    {
        public ImageType ImageType { get; set; }
        public FileInfo FileInfo { get; set; }
        public byte[] Md5CheckSum { get; set; }
        public int FileIndex { get; set; }
    }
}