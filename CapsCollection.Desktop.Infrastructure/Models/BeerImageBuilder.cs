using System;
using System.Globalization;
using System.Windows.Media.Imaging;
using CapsCollection.Common.Settings;
using CapsCollection.Desktop.Infrastructure.Extensions;

namespace CapsCollection.Desktop.Infrastructure.Models
{
    public class BeerImageBuilder
    {
        const string EMPTY_CAP_URI_STRING = "/CapsCollection.Desktop.Infrastructure;component/Resources/Images/EmptyCap.png";
        const string EMPTY_BOTTLE_URI_STRING = "/CapsCollection.Desktop.Infrastructure;component/Resources/Images/EmptyBottle.png";
        const string EMPTY_LABEL_URI_STRING = "/CapsCollection.Desktop.Infrastructure;component/Resources/Images/EmptyLabel.png";

        private string _imageTypeString;
        private string _emptyUriSrting;
        private string _beerNumber;
        private Uri _blobStorageUri;

        private byte[] _fullSizeBytes;
        private byte[] _thumbnailBytes;
        private byte[] _previewBytes;

        private string _fileName;
        private string _previewFileName;
        private string _thumbnailFileName;

        private Uri _fullSizeUri;
        private Uri _previewUri;
        private Uri _thumbnailUri;

        private BitmapImage _previewImage;
        private BitmapImage _thumbnailImage;

        private string _imageContainer;
        
        private string _fileNameTemplate;
        private string _previewFileNameTemplate;
        private string _thumbnailFileNameTemplate;

        private string _sourceFileFullPath;

        private BeerImageBuilder(ImageType imageType)
        {
            _blobStorageUri = CapsCollectionAzureSettings.BlobStorageUrl;

            switch (imageType)
            {
                case ImageType.Bottle:
                    _imageTypeString = "bottle";
                    _imageContainer = CapsCollectionAzureSettings.BottlesContainer;
                    _emptyUriSrting = EMPTY_BOTTLE_URI_STRING;
                    break;
                case ImageType.Cap:
                    _imageTypeString = "cap";
                    _imageContainer = CapsCollectionAzureSettings.CapsContainer;
                    _emptyUriSrting = EMPTY_CAP_URI_STRING;
                    break;
                case ImageType.Label:
                    _imageTypeString = "label";
                    _imageContainer = CapsCollectionAzureSettings.LabelsContainer;
                    _emptyUriSrting = EMPTY_LABEL_URI_STRING;
                    break;
            }

            _fullSizeBytes = new byte[] { };
            _thumbnailBytes = new byte[] { };
            _previewBytes = new byte[] { };


            _fileNameTemplate = String.Format(@"{0}_{1}.png", _imageTypeString, "{0}");
            _previewFileNameTemplate = String.Format(@"{0}_{1}_preview.png", _imageTypeString, "{0}");
            _thumbnailFileNameTemplate = String.Format(@"{0}_{1}_thumbnail.png", _imageTypeString, "{0}");
        }
        
        public static BeerImageBuilder CreateBuilder(ImageType imageType)
        {
            return new BeerImageBuilder(imageType);
        }

        public BeerImageBuilder AttachThumbnails(ImageDataWithThumbnails imageDataWithThumbnails)
        {
            if (imageDataWithThumbnails != null)
            {
                _fullSizeBytes = imageDataWithThumbnails.HiResBytes;
                _thumbnailBytes = imageDataWithThumbnails.ThumbnailBytes;
                _previewBytes = imageDataWithThumbnails.PreviewBytes;
                _sourceFileFullPath = imageDataWithThumbnails.SourceFileFullPath;
            }

            return this;
        }
        
        public BeerImageBuilder AttachUri(int beerIndex)
        {
            if (beerIndex == 0)
            {
                return this;
            }

            _beerNumber = beerIndex.ToString(CultureInfo.InvariantCulture).PadLeft(5, '0');

            _fileName = String.Format(_fileNameTemplate, _beerNumber);
            _previewFileName = String.Format(_previewFileNameTemplate, _beerNumber);
            _thumbnailFileName = String.Format(_thumbnailFileNameTemplate, _beerNumber);

            _fullSizeUri = new Uri(_blobStorageUri, String.Format(@"{0}/{1}", _imageContainer, _fileName));
            _previewUri = new Uri(_blobStorageUri, String.Format(@"{0}/{1}", _imageContainer, _previewFileName));
            _thumbnailUri = new Uri(_blobStorageUri, String.Format(@"{0}/{1}", _imageContainer, _thumbnailFileName));

            _previewImage = new BitmapImage(_previewUri);
            _thumbnailImage = new BitmapImage(_thumbnailUri);

            return this;
        }

        public static implicit operator BeerImage(BeerImageBuilder builder)
        {
            BeerImage beerImage = new BeerImage();

            beerImage.FullSizeBytes = builder._fullSizeBytes;
            beerImage.PreviewBytes = builder._previewBytes;
            beerImage.ThumbnailBytes = builder._thumbnailBytes;

            beerImage.FullSize = builder._fullSizeBytes.ToReadaleSize();
            beerImage.PreviewSize = builder._previewBytes.ToReadaleSize();
            beerImage.ThumbnailSize = builder._thumbnailBytes.ToReadaleSize();

            beerImage.EmptyImageUri = new Uri(builder._emptyUriSrting, UriKind.Relative);

            beerImage.PreviewImage = builder._previewImage;
            beerImage.ThumbnailImage = builder._thumbnailImage;

            beerImage.FileName = builder._fileName;
            beerImage.PreviewFileName = builder._previewFileName;
            beerImage.ThumbnailFileName = builder._thumbnailFileName;

            beerImage.FullSizeUri = builder._fullSizeUri;
            beerImage.PreviewUri = builder._previewUri;
            beerImage.ThumbnailUri = builder._thumbnailUri;

            beerImage.Container = builder._imageContainer;

            beerImage.FileNameTemplate = builder._fileNameTemplate;
            beerImage.PreviewFileNameTemplate = builder._previewFileNameTemplate;
            beerImage.ThumbnailFileNameTemplate = builder._thumbnailFileNameTemplate;

            beerImage.SourceFileFullPath = builder._sourceFileFullPath;

            return beerImage;
        }
    }
}