using System;
using System.Linq;
using System.Windows.Media.Imaging;

namespace CapsCollection.Desktop.Infrastructure.Models
{
    public class BeerImage
    {
        public BitmapImage ThumbnailImage { get; set; }
        public BitmapImage PreviewImage { get; set; }

        public byte[] FullSizeBytes { get; set; }
        public byte[] PreviewBytes { get; set; }
        public byte[] ThumbnailBytes { get; set; }

        public string FileName { get; set; }
        public string PreviewFileName { get; set; }
        public string ThumbnailFileName { get; set; }

        public Uri EmptyImageUri { get; set; }

        public Uri FullSizeUri { get; set; }
        public Uri PreviewUri { get; set; }
        public Uri ThumbnailUri { get; set; }

        public string FullSize { get; set; }
        public string PreviewSize { get; set; }
        public string ThumbnailSize { get; set; }
        public string Container { get; set; }

        public string FileNameTemplate { get; set; }
        public string PreviewFileNameTemplate { get; set; }
        public string ThumbnailFileNameTemplate { get; set; }

        public string SourceFileFullPath { get; set; }

        public override bool Equals(object obj)
        {
            BeerImage beerImage = obj as BeerImage;
            if (beerImage == null)
            {
                return false;
            }

            return beerImage.FullSizeBytes.SequenceEqual(FullSizeBytes) &&
                   beerImage.PreviewBytes.SequenceEqual(PreviewBytes) &&
                   beerImage.ThumbnailBytes.SequenceEqual(ThumbnailBytes);
        }

        public override int GetHashCode()
        {
            int hashCode = (FullSizeBytes != null ? FullSizeBytes.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (PreviewBytes != null ? PreviewBytes.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (ThumbnailBytes != null ? ThumbnailBytes.GetHashCode() : 0);
            return hashCode;
        }
    }
}
