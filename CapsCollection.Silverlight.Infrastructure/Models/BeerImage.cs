using CapsCollection.Silverlight.Infrastructure.Helpers;
using System;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using CapsCollection.Common.Settings;

namespace CapsCollection.Silverlight.Infrastructure.Models
{
    public class BeerImage : INotifyPropertyChanged
    {
        const string EMPTY_CAP_URI_STRING = "/Resources/Images/EmptyCap.png";
        const string EMPTY_BOTTLE_URI_STRING = "/Resources/Images/EmptyBottle.png";
        const string EMPTY_LABEL_URI_STRING = "/Resources/Images/EmptyLabel.png";

        public BeerImage(BeerImageType beerImageType)
        {
            ThumbnailImage = new BitmapImage();
            PreviewImage = new BitmapImage();

            DefineUriToEmptyImage(beerImageType);
        }

        public BeerImage(BeerImageType beerImageType, int beerId)
            : this(beerImageType)
        {
            _beerId = beerId;
            if (beerId != 0)
            {
                DefineBeerFileNames(beerId, beerImageType);
            }
        }

        private int _beerId;
        public int BeerId
        {
            get
            {
                return _beerId;
            }
            set
            {
                _beerId = value;
                RaisePropertyChanged("BeerId");
            }
        }

        private BitmapImage _thumbnailImage;
        public BitmapImage ThumbnailImage
        {
            get
            {
                return _thumbnailImage;
            }
            set
            {
                _thumbnailImage = value;
                RaisePropertyChanged("ThumbnailImage");
            }
        }

        private bool _isTumbnailImageLoading;
        public bool IsThumbnailImageLoading
        {
            get
            {
                return _isTumbnailImageLoading;
            }
            set
            {
                _isTumbnailImageLoading = value;
                RaisePropertyChanged("IsThumbnailImageLoading");
            }
        }

        private BitmapImage _previewImage;
        public BitmapImage PreviewImage
        {
            get
            {
                return _previewImage;
            }
            set
            {
                _previewImage = value;
                RaisePropertyChanged("PreviewImage");
            }
        }

        private bool _isPreviewImageLoading;
        public bool IsPreviewImageLoading
        {
            get
            {
                return _isPreviewImageLoading;
            }
            set
            {
                _isPreviewImageLoading = value;
                RaisePropertyChanged("IsPreviewImageLoading");
            }
        }

        private int _previewDownloadProgress;
        public int PreviewDownloadProgress
        {
            get
            {
                return _previewDownloadProgress;
            }
            set
            {
                _previewDownloadProgress = value;
                PreviewDownloadProgressText = string.Format("{0}% done", value);
                RaisePropertyChanged("PreviewDownloadProgress");
            }
        }

        private string _previewDownloadProgressText;
        public string PreviewDownloadProgressText
        {
            get
            {
                return _previewDownloadProgressText;
            }
            set
            {
                _previewDownloadProgressText = value;
                RaisePropertyChanged("PreviewDownloadProgressText");
            }
        }

        public byte[] ImageHiResBytes { get; set; }
        public byte[] ImagePreviewBytes { get; set; }
        public byte[] ImageThumbnailBytes { get; set; }

        public string ImageContainerPath { get; set; }

        public string ImageHiResFileName { get; set; }
        public string ImagePreviewFileName { get; set; }
        public string ImageThumbnailFileName { get; set; }

        public Uri EmptyImageUri { get; set; }

        public Uri ImageHiResUri { get; set; }
        public Uri ImagePreviewUri { get; set; }
        public Uri ImageThumbnailUri { get; set; }
        
        private void DefineBeerFileNames(int id, BeerImageType imageType)
        {
            var beerNumber = id.ToString().PadLeft(5, '0');

            string imageTypeString = string.Empty;
            switch (imageType)
            {
                case BeerImageType.Bottle:
                    imageTypeString = "bottle";
                    break;
                case BeerImageType.Cap:
                    imageTypeString = "cap";
                    break;
                case BeerImageType.Label:
                    imageTypeString = "label";
                    break;
            }

            var imageHiResFileName = string.Format(@"{0}_{1}{2}.png", imageTypeString, beerNumber, string.Empty);
            var imagePreviewFileName = string.Format(@"{0}_{1}{2}.png", imageTypeString, beerNumber, "_preview");
            var imageThumbnailFileName = string.Format(@"{0}_{1}{2}.png", imageTypeString, beerNumber, "_thumbnail");

            ImageHiResFileName = imageHiResFileName;
            ImagePreviewFileName = imagePreviewFileName;
            ImageThumbnailFileName = imageThumbnailFileName;

            var blobStorageUri = CapsCollectionAzureSettings.BlobStorageUrl;

            ImageHiResUri = new Uri(blobStorageUri, String.Format("{0}/{1}", ImageContainerPath, imageHiResFileName));
            ImagePreviewUri = new Uri(blobStorageUri, String.Format("{0}/{1}", ImageContainerPath, imagePreviewFileName));
            ImageThumbnailUri = new Uri(blobStorageUri, String.Format("{0}/{1}", ImageContainerPath, imageThumbnailFileName));
        }
        
        public void DefineUriToEmptyImage(BeerImageType imageType)
        {
            string imageFolder = string.Empty;
            string uriSrting = string.Empty;
            switch (imageType)
            {
                case BeerImageType.Bottle:
                    uriSrting = EMPTY_BOTTLE_URI_STRING;
                    imageFolder = CapsCollectionAzureSettings.BottlesContainer;
                    break;
                case BeerImageType.Cap:
                    uriSrting = EMPTY_CAP_URI_STRING;
                    imageFolder = CapsCollectionAzureSettings.CapsContainer;
                    break;
                case BeerImageType.Label:
                    uriSrting = EMPTY_LABEL_URI_STRING;
                    imageFolder = CapsCollectionAzureSettings.LabelsContainer;
                    break;
            }

            var emptyUri = new Uri(uriSrting, UriKind.Relative);

            EmptyImageUri = emptyUri;
            ImageContainerPath = imageFolder;
        }

        public void ClearImage()
        {
            ImageHiResBytes = null;
            ImagePreviewBytes = null;
            ImageThumbnailBytes = null;

            ImageHiResUri = null;
            ImagePreviewUri = null;
            ImageThumbnailUri = null;

            GetEmptyImage(BitmapCreateOptions.IgnoreImageCache);
        }

        public void GetThumbnailImage(BitmapCreateOptions createOptions)
        {
            if (_beerId != 0)
            {
                // Show busy status.
                IsThumbnailImageLoading = true;

                // Process thumbnail loading.
                ThumbnailImage.ImageOpened += (s, e) =>
                {
                    // When loaded, then hide progress information.
                    IsThumbnailImageLoading = false;
                };
                ThumbnailImage.ImageFailed += (s, e) =>
                {
                    // If load fails, then show nothing.
                    IsThumbnailImageLoading = false;
                    ThumbnailImage.UriSource = null;
                };
                ThumbnailImage.CreateOptions = createOptions;
                ThumbnailImage.UriSource = ImageThumbnailUri;
            }
        }

        public void GetEmptyImage(BitmapCreateOptions createOptions)
        {
            // Show busy status.
            IsThumbnailImageLoading = true;

            // Process thumbnail loading.
            ThumbnailImage.ImageOpened += (s, e) =>
            {
                // When loaded, then hide progress information.
                IsThumbnailImageLoading = false;
            };
            ThumbnailImage.ImageFailed += (s, e) =>
            {
                // If load fails, then show nothing.
                IsThumbnailImageLoading = false;
            };
            ThumbnailImage.CreateOptions = createOptions;
            ThumbnailImage.UriSource = EmptyImageUri;

        }

        public void GetThumbnailImageWithFallback(BitmapCreateOptions createOptions)
        {
            if (ImageThumbnailUri == null)
                ImageThumbnailUri = EmptyImageUri;

            // Show busy status.
            IsThumbnailImageLoading = true;

            // Process thumbnail loading.
            ThumbnailImage.ImageOpened += (s, e) =>
            {
                // When loaded, then hide progress information.
                IsThumbnailImageLoading = false;
            };
            ThumbnailImage.ImageFailed += (s, e) =>
            {
                // If load fails, then show nothing.
                IsThumbnailImageLoading = false;
                ThumbnailImage.UriSource = EmptyImageUri;
            };
            ThumbnailImage.CreateOptions = createOptions;
            ThumbnailImage.UriSource = ImageThumbnailUri;
        }

        public void GetImagePreview(BitmapCreateOptions createOptions)
        {
            // Refresh selected preview information and show busy status.
            PreviewDownloadProgress = 0;
            IsPreviewImageLoading = true;

            // Process preview loading.
            _previewImage.ImageOpened += (s, e) =>
            {
                // When loaded, then hide progress information.
                IsPreviewImageLoading = false;
            };
            _previewImage.DownloadProgress += (s, e) =>
            {
                // Show download progress information.
                var progress = e.Progress;
                PreviewDownloadProgress = progress;
            };
            _previewImage.ImageFailed += (s, e) =>
            {
                // If load fails, then show empty image.
                IsPreviewImageLoading = false;
                _previewImage.UriSource = EmptyImageUri;
            };
            _previewImage.CreateOptions = createOptions;
            _previewImage.UriSource = ImagePreviewUri;
        }

        public void CreateThumbnails(BeerImageType imageType)
        {
            switch (imageType)
            {
                case BeerImageType.Bottle:
                    ImageHiResBytes = ImageHiResBytes;
                    ImagePreviewBytes = ThumbnailGenerator.Generate(ImageHiResBytes, 60, 173);
                    ImageThumbnailBytes = ThumbnailGenerator.Generate(ImageHiResBytes, 45, 130);
                    break;
                case BeerImageType.Cap:
                    ImageHiResBytes = ImageHiResBytes;
                    ImagePreviewBytes = ThumbnailGenerator.Generate(ImageHiResBytes, 300, 300);
                    ImageThumbnailBytes = ThumbnailGenerator.Generate(ImageHiResBytes, 100, 100);
                    break;
                case BeerImageType.Label:
                    ImageHiResBytes = ImageHiResBytes;
                    ImagePreviewBytes = ThumbnailGenerator.Generate(ImageHiResBytes, 300, 300);
                    ImageThumbnailBytes = ThumbnailGenerator.Generate(ImageHiResBytes, 100, 100);
                    break;
            }
        }

        public void CreateThumbnails(BeerImageType imageType, int beerId)
        {
            if (beerId != 0)
            {
                DefineBeerFileNames(beerId, imageType);
            }
            switch (imageType)
            {
                case BeerImageType.Bottle:
                    ImageHiResBytes = ImageHiResBytes;
                    ImagePreviewBytes = ThumbnailGenerator.Generate(ImageHiResBytes, 60, 173);
                    ImageThumbnailBytes = ThumbnailGenerator.Generate(ImageHiResBytes, 45, 130);
                    break;
                case BeerImageType.Cap:
                    ImageHiResBytes = ImageHiResBytes;
                    ImagePreviewBytes = ThumbnailGenerator.Generate(ImageHiResBytes, 300, 300);
                    ImageThumbnailBytes = ThumbnailGenerator.Generate(ImageHiResBytes, 100, 100);
                    break;
                case BeerImageType.Label:
                    ImageHiResBytes = ImageHiResBytes;
                    ImagePreviewBytes = ThumbnailGenerator.Generate(ImageHiResBytes, 300, 300);
                    ImageThumbnailBytes = ThumbnailGenerator.Generate(ImageHiResBytes, 100, 100);
                    break;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler propertyChanged = PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
