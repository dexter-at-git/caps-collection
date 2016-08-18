using System;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using CapsCollection.Common.Settings;
using CapsCollection.Silverlight.ServiceAgents.Proxies.Geography;

namespace CapsCollection.Silverlight.Infrastructure.Models
{
    public class CountryWithFlags : INotifyPropertyChanged
    {
        private const string EMPTY_FLAG_URI_STRING = "/Resources/Images/EmptyFlag.png";
        
        public CountryWithFlags()
        {
            EmptyFlagUri = new Uri(EMPTY_FLAG_URI_STRING, UriKind.Relative);

            _flagFullImage = new BitmapImage();
            _flagRoundImage = new BitmapImage();
            _flagSquareImage = new BitmapImage();
        }

        public int CountryId { get; set; }
        public int ContinentId { get; set; }
        public ContinentDto Continent { get; set; }
        
        public string EnglishCountryName { get; set; }
        public string EnglishCountryFullName { get; set; }
        public string NationalCountryName { get; set; }
        public string NationalCountryFullName { get; set; }
        
        public string Alpha2 { get; set; }
        public string Alpha3 { get; set; }
        public string ISO { get; set; }
        public string PreciseLocation { get; set; }
        private Uri EmptyFlagUri { get; set; }
        public byte[] FlagFullBytes { get; set; }
        public byte[] FlagRoundBytes { get; set; }
        public byte[] FlagSquareBytes { get; set; }
        public int BeerCount { get; set; }
        
        private BitmapImage _flagFullImage;
        public BitmapImage FlagFullImage
        {
            get
            {
                return _flagFullImage;
            }
            set
            {
                _flagFullImage = value;
                RaisePropertyChanged("FlagFullImage");
            }
        }
        
        private BitmapImage _flagRoundImage;
        public BitmapImage FlagRoundImage
        {
            get
            {
                return _flagRoundImage;
            }
            set
            {
                _flagRoundImage = value;
                RaisePropertyChanged("FlagRoundImage");
            }
        }
        
        private BitmapImage _flagSquareImage;
        public BitmapImage FlagSquareImage
        {
            get
            {
                return _flagSquareImage;
            }
            set
            {
                _flagSquareImage = value;
                RaisePropertyChanged("FlagSquareImage");
            }
        }

        private Uri CreateFlagUri(FlagType flagType)
        {
            if (Alpha3 == null ||  Alpha3.Trim() == string.Empty)
                return new Uri(EMPTY_FLAG_URI_STRING, UriKind.Relative);
            
            var blobStorageUri = CapsCollectionAzureSettings.BlobStorageUrl;
            var flagComtainer = CapsCollectionAzureSettings.FlagsContainer; 
            var flag = string.Empty;

            switch (flagType)
            {
                case FlagType.Full:
                    flag = "full";
                    break;
                case FlagType.Round:
                    flag = "round";
                    break;
                case FlagType.Square:
                    flag = "square";
                    break;
            }

            var flagUri = new Uri(blobStorageUri, String.Format("{0}/{1}_{2}.png", flagComtainer, Alpha3, flag));

            return flagUri;
        }
        
        public void CreateFlagFullImage(BitmapCreateOptions createOptions, FlagType flagType)
        {
            // Create uri for bitmap image.
            var uri = CreateFlagUri(flagType);

            // Create bitmap image.
            _flagFullImage.ImageFailed += (s, e) =>
            {
                _flagFullImage.UriSource = EmptyFlagUri;
            };
            _flagFullImage.CreateOptions = createOptions;
            _flagFullImage.UriSource = uri;
        }
        
        public void CreateFlagRoundImage(BitmapCreateOptions createOptions, FlagType flagType)
        {
            // Create uri for bitmap image.
            var uri = CreateFlagUri(flagType);

            // Create bitmap image.
            _flagRoundImage.ImageFailed += (s, e) =>
            {
                _flagRoundImage.UriSource = EmptyFlagUri;
            };
            _flagRoundImage.CreateOptions = createOptions;
            _flagRoundImage.UriSource = uri;
        }
        
        public void CreateFlagSqaureImage(BitmapCreateOptions createOptions, FlagType flagType)
        {
            // Create uri for bitmap image.
            var uri = CreateFlagUri(flagType);

            // Create bitmap image.
            _flagSquareImage.ImageFailed += (s, e) =>
            {
                _flagSquareImage.UriSource = EmptyFlagUri;
            };
            _flagSquareImage.CreateOptions = createOptions;
            _flagSquareImage.UriSource = uri;
        }
        
        public void ClearImage(FlagType flagType)
        {
            switch (flagType)
            {
                case FlagType.Full:
                    _flagFullImage.UriSource = EmptyFlagUri;
                    break;
                case FlagType.Round:
                    _flagRoundImage.UriSource = EmptyFlagUri;
                    break;
                case FlagType.Square:
                    _flagSquareImage.UriSource = EmptyFlagUri;
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
