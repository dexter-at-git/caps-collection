namespace CapsCollection.Desktop.Infrastructure.Models
{
    public class ImageDataWithThumbnails
    {
        public byte[] HiResBytes { get; set; }
        public byte[] PreviewBytes { get; set; }
        public byte[] ThumbnailBytes { get; set; }

        public ImageType ImageType { get; set; }
        public int FileIndex { get; set; }
        public string SourceFileFullPath { get; set; }
    }
}