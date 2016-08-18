using CapsCollection.Silverlight.Infrastructure.Extensions;
using CapsCollection.Silverlight.Infrastructure.Models;
using CapsCollection.Silverlight.Infrastructure.ViewModels;
using CapsCollection.Silverlight.ServiceAgents.Proxies.Geography;
using CapsCollection.Silverlight.UI.Modules.Geography.Validators;
using CapsCollection.Silverlight.UI.Modules.Geography.Views;
using FluentValidation;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using System.Windows.Media.Imaging;
using CapsCollection.Silverlight.Infrastructure.Events;
using CapsCollection.Silverlight.ServiceAgents.ProxiesWrappers;
using CapsCollection.Silverlight.UI.Modules.Services.Interfaces;

namespace CapsCollection.Silverlight.UI.Modules.Geography.ViewModels
{
    public class CountryEditViewModel : ViewModelBase, INavigationAware
    {
        #region MEF imports

        [Import]
        public IRegionManager RegionManager { get; set; }
        [Import]
        public IEventAggregator EventAggregator { get; set; }
        [Import]
        public IAuthenticationManager AuthenticationManager { get; set; }

        #endregion


        #region Commands and Events

        public DelegateCommand SaveCommand { get; private set; }
        public DelegateCommand CloseCommand { get; private set; }
        public DelegateCommand ClearFlagCommand { get; private set; }
        public DelegateCommand ClearFlagRoundCommand { get; private set; }
        public DelegateCommand ClearFlagSquareCommand { get; private set; }

        #endregion


        #region Properties and Members

        private readonly CountryModelValidator _validator;

        public InteractionRequest<Notification> ShowMessagebox { get; set; }


        public string Title
        {
            get { return _country != null ? _country.EnglishCountryName : "Empty"; }
        }

        private bool _isDirty;
        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                _isDirty = value;
                SaveCommand.RaiseCanExecuteChanged();
            }
        }


        private ObservableCollection<ContinentDto> _continents;
        public ObservableCollection<ContinentDto> Continents
        {
            get { return _continents; }
            set
            {
                _continents.Clear();
                _continents = value;
                RaisePropertyChanged(() => Continents);
            }
        }


        private ContinentDto _selectedContinent;
        public ContinentDto SelectedContinent
        {
            get { return _selectedContinent; }
            set
            {
                if (_selectedContinent != value)
                {
                    _selectedContinent = value;

                    if (_selectedContinent != null)
                    {
                        _country.ContinentId = _selectedContinent.ContinentId;
                    }
                    IsDirty = true;
                    RaisePropertyChanged(() => SelectedContinent);
                }
            }
        }

        private CountryWithFlags _country;
        public CountryWithFlags Country
        {
            get { return _country; }
            set
            {
                if (_country != value)
                {
                    RaisePropertyChanged(() => Country);
                    RaisePropertyChanged(() => Title);
                }
            }
        }

        private string _englishCountryName;
        public string EnglishCountryName
        {
            get { return _englishCountryName; }
            set
            {
                if (_englishCountryName != value)
                {
                    _englishCountryName = value;
                    Country.EnglishCountryName = value;
                    RaisePropertyChanged(() => EnglishCountryName);
                }

                ClearErrorFromProperty("EnglishCountryName");
                var validationResult = _validator.Validate(Country, "EnglishCountryName");
                if (!validationResult.IsValid)
                    validationResult.Errors.ToList().ForEach(x => AddErrorForProperty(x.PropertyName, new ErrorInfo { ErrorMessage = x.ErrorMessage, IsWarning = false }));

                IsDirty = true;
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        private string _englishCountryFullName;
        public string EnglishCountryFullName
        {
            get { return _englishCountryFullName; }
            set
            {
                if (_englishCountryFullName != value)
                {
                    _englishCountryFullName = value;
                    Country.EnglishCountryFullName = value;
                    RaisePropertyChanged(() => EnglishCountryFullName);
                }

                ClearErrorFromProperty("EnglishCountryFullName");
                var validationResult = _validator.Validate(Country, "EnglishCountryFullName");
                if (!validationResult.IsValid)
                    validationResult.Errors.ToList().ForEach(x => AddErrorForProperty(x.PropertyName, new ErrorInfo { ErrorMessage = x.ErrorMessage, IsWarning = false }));

                IsDirty = true;
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        private string _nationalCountryName;
        public string NationalCountryName
        {
            get { return _nationalCountryName; }
            set
            {
                if (_nationalCountryName != value)
                {
                    _nationalCountryName = value;
                    Country.NationalCountryName = value;
                    RaisePropertyChanged(() => NationalCountryName);
                }

                ClearErrorFromProperty("NationalCountryName");
                var validationResult = _validator.Validate(Country, "NationalCountryName");
                if (!validationResult.IsValid)
                    validationResult.Errors.ToList().ForEach(x => AddErrorForProperty(x.PropertyName, new ErrorInfo { ErrorMessage = x.ErrorMessage, IsWarning = false }));

                IsDirty = true;
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        private string _nationalCountryFullName;
        public string NationalCountryFullName
        {
            get { return _nationalCountryFullName; }
            set
            {
                if (_nationalCountryFullName != value)
                {
                    _nationalCountryFullName = value;
                    Country.NationalCountryFullName = value;
                    RaisePropertyChanged(() => NationalCountryFullName);
                }

                ClearErrorFromProperty("NationalCountryFullName");
                var validationResult = _validator.Validate(Country, "NationalCountryFullName");
                if (!validationResult.IsValid)
                    validationResult.Errors.ToList().ForEach(x => AddErrorForProperty(x.PropertyName, new ErrorInfo { ErrorMessage = x.ErrorMessage, IsWarning = false }));

                IsDirty = true;
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        private string _alpha2;
        public string Alpha2
        {
            get { return _alpha2; }
            set
            {
                if (_alpha2 != value)
                {
                    _alpha2 = value;
                    Country.Alpha2 = value;
                    RaisePropertyChanged(() => Alpha2);
                }

                ClearErrorFromProperty("Alpha2");
                var validationResult = _validator.Validate(Country, "Alpha2");
                if (!validationResult.IsValid)
                    validationResult.Errors.ToList().ForEach(x => AddErrorForProperty(x.PropertyName, new ErrorInfo { ErrorMessage = x.ErrorMessage, IsWarning = false }));

                IsDirty = true;
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        private string _alpha3;
        public string Alpha3
        {
            get { return _alpha3; }
            set
            {
                if (_alpha3 != value)
                {
                    _alpha3 = value;
                    Country.Alpha3 = value;
                    RaisePropertyChanged(() => Alpha3);
                }

                ClearErrorFromProperty("Alpha3");
                var validationResult = _validator.Validate(Country, "Alpha3");
                if (!validationResult.IsValid)
                    validationResult.Errors.ToList().ForEach(x => AddErrorForProperty(x.PropertyName, new ErrorInfo { ErrorMessage = x.ErrorMessage, IsWarning = false }));

                IsDirty = true;
                SaveCommand.RaiseCanExecuteChanged();
            }
        }
        private string OldAlpha3 { get; set; }

        private string _iso;
        public string Iso
        {
            get { return _iso; }
            set
            {
                if (_iso != value)
                {
                    _iso = value;
                    Country.ISO = value;
                    RaisePropertyChanged(() => Iso);
                }

                ClearErrorFromProperty("Iso");
                var validationResult = _validator.Validate(Country, "Iso");
                if (!validationResult.IsValid)
                    validationResult.Errors.ToList().ForEach(x => AddErrorForProperty(x.PropertyName, new ErrorInfo { ErrorMessage = x.ErrorMessage, IsWarning = false }));

                IsDirty = true;
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        private string _preciseLocation;
        public string PreciseLocation
        {
            get { return _preciseLocation; }
            set
            {
                if (_preciseLocation != value)
                {
                    _preciseLocation = value;
                    Country.PreciseLocation = value;
                    RaisePropertyChanged(() => PreciseLocation);
                }

                ClearErrorFromProperty("PreciseLocation");
                var validationResult = _validator.Validate(Country, "PreciseLocation");
                if (!validationResult.IsValid)
                    validationResult.Errors.ToList().ForEach(x => AddErrorForProperty(x.PropertyName, new ErrorInfo { ErrorMessage = x.ErrorMessage, IsWarning = false }));

                IsDirty = true;
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion


        #region Image properties

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

        private byte[] _flagFullImageBytes;
        public byte[] FlagFullImageBytes
        {
            get
            {
                return _flagFullImageBytes;
            }
            set
            {
                if (_flagFullImageBytes == value) return;
                _flagFullImageBytes = value;

                if (value != null)
                {
                    using (var ms = new MemoryStream(value))
                    {
                        FlagFullImage = new BitmapImage();
                        FlagFullImage.SetSource(ms);
                    }
                }

                RaisePropertyChanged(() => FlagFullImageBytes);

                IsDirty = true;
                SaveCommand.RaiseCanExecuteChanged();
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

        private byte[] _flagRoundImageBytes;
        public byte[] FlagRoundImageBytes
        {
            get
            {
                return _flagRoundImageBytes;
            }
            set
            {
                if (_flagRoundImageBytes == value) return;
                _flagRoundImageBytes = value;

                if (value != null)
                {
                    using (var ms = new MemoryStream(value))
                    {
                        FlagRoundImage = new BitmapImage();
                        FlagRoundImage.SetSource(ms);
                    }
                }

                RaisePropertyChanged(() => FlagRoundImageBytes);

                IsDirty = true;
                SaveCommand.RaiseCanExecuteChanged();
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

        private byte[] _flagSquareImageBytes;
        public byte[] FlagSquareImageBytes
        {
            get
            {
                return _flagSquareImageBytes;
            }
            set
            {
                if (_flagSquareImageBytes == value) return;
                _flagSquareImageBytes = value;

                if (value != null)
                {
                    using (var ms = new MemoryStream(value))
                    {
                        FlagSquareImage = new BitmapImage();
                        FlagSquareImage.SetSource(ms);
                    }
                }

                RaisePropertyChanged(() => FlagSquareImageBytes);

                IsDirty = true;
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion


        #region Constructors

        public CountryEditViewModel(CountryModelValidator validator)
        {
            if (validator == null)
                throw new ArgumentNullException("validator");

            _validator = validator;

            _continents = new ObservableCollection<ContinentDto>();

            _flagFullImage = new BitmapImage();
            _flagRoundImage = new BitmapImage();
            _flagSquareImage = new BitmapImage();

            // Commands.
            SaveCommand = new DelegateCommand(OnSave, CanSave);
            CloseCommand = new DelegateCommand(OnClose);
            ClearFlagCommand = new DelegateCommand(OnClearFlag);
            ClearFlagRoundCommand = new DelegateCommand(OnClearFlagRound);
            ClearFlagSquareCommand = new DelegateCommand(OnClearFlagSquare);

            // Poopup message boxes.
            ShowMessagebox = new InteractionRequest<Notification>();

            if (!DesignerProperties.IsInDesignTool)
            {
                CompositionInitializer.SatisfyImports(this);
            }
        }

        #endregion


        #region Buttons click command methods

        private bool CanSave()
        {
            IsAuthenticated = AuthenticationManager.AuthenticationInfo.IsAuthenticated;
            return IsDirty && !HasErrors && IsAuthenticated;
        }

        private void OnSave()
        {
            IsDirty = false;
            SaveCountry();
        }

        private void OnClose()
        {
            IRegion editCountryContentRegion = RegionManager.Regions["PopupRegionContent"];
            foreach (var view in editCountryContentRegion.Views)
            {
                if (view is CountryEditView && ((FrameworkElement)view).DataContext == this)
                {
                    editCountryContentRegion.Remove(view);
                    break;
                }
            }
        }

        private void OnClearFlag()
        {
            _flagFullImageBytes = null;
            _country.ClearImage(FlagType.Full);
            FlagFullImage = _country.FlagFullImage;
            IsDirty = true;
        }

        private void OnClearFlagRound()
        {
            _flagRoundImageBytes = null;
            _country.ClearImage(FlagType.Round);
            FlagRoundImage = _country.FlagRoundImage;
            IsDirty = true;
        }

        private void OnClearFlagSquare()
        {
            _flagSquareImageBytes = null;
            _country.ClearImage(FlagType.Square);
            FlagSquareImage = _country.FlagSquareImage;
            IsDirty = true;
        }

        #endregion


        #region Methods to communicate with database

        public void GetContinents(int continentId)
        {
            var client = new GeographyServiceClientWrapper();

            client.GetContinentsAsync();
            client.GetContinentsCompleted += delegate (object sender, GetContinentsCompletedEventArgs e)
            {
                if (e.Error == null)
                {
                    var continents = e.Result;

                    Continents = continents;
                    SelectedContinent = _continents.FirstOrDefault(c => c.ContinentId == continentId);
                }
                else if (e.Error is FaultException<ExceptionDetail>)
                {
                    var serviceFault = e.Error as FaultException<ExceptionDetail>;
                    
                    ShowMessagebox.Raise(new Notification
                    {
                        Title = "FaultException. Error loading continents.",
                        Content = serviceFault.Detail.Message
                    }, notification => { });
                }
                else if (e.Error is CommunicationException)
                {
                    ShowMessagebox.Raise(new Notification
                    {
                        Title = "Exception.",
                        Content = "Cannot reach the service."
                    }, notification => { });
                }
                else if (e.Error != null)
                {
                    ShowMessagebox.Raise(new Notification
                    {
                        Title = "Exception. Error loading continents.",
                        Content = e.Error.Message
                    }, notification => { });
                }
            };
        }

        public void GetCountry(int countryId)
        {
            var client = new GeographyServiceClientWrapper();

            client.GetCountryAsync(countryId);
            client.GetCountryCompleted += delegate (object sender, GetCountryCompletedEventArgs e)
            {
                if (e.Error == null)
                {
                    var country = e.Result;

                    _country = new CountryWithFlags
                    {
                        CountryId = countryId,
                        ContinentId = e.Result.ContinentId,
                        EnglishCountryName = e.Result.EnglishCountryName,
                        EnglishCountryFullName = e.Result.EnglishCountryFullName,
                        NationalCountryName = e.Result.NationalCountryName,
                        NationalCountryFullName = e.Result.NationalCountryFullName,
                        Alpha2 = e.Result.Alpha2,
                        Alpha3 = e.Result.Alpha3,
                        ISO = e.Result.ISO,
                        PreciseLocation = e.Result.PreciseLocation
                    };

                    // Create images
                    _country.CreateFlagFullImage(BitmapCreateOptions.IgnoreImageCache, FlagType.Full);
                    _country.CreateFlagRoundImage(BitmapCreateOptions.IgnoreImageCache, FlagType.Round);
                    _country.CreateFlagSqaureImage(BitmapCreateOptions.IgnoreImageCache, FlagType.Square);
                    
                    SelectedContinent = _continents.FirstOrDefault(c => c.ContinentId == country.ContinentId);
                    EnglishCountryName = _country.EnglishCountryName;
                    EnglishCountryFullName = _country.EnglishCountryFullName;
                    NationalCountryName = _country.NationalCountryName;
                    NationalCountryFullName = _country.NationalCountryFullName;
                    Alpha2 = _country.Alpha2;
                    Alpha3 = _country.Alpha3;
                    OldAlpha3 = country.Alpha3;
                    Iso = _country.ISO;
                    PreciseLocation = _country.PreciseLocation;

                    FlagFullImage = _country.FlagFullImage;
                    FlagRoundImage = _country.FlagRoundImage;
                    FlagSquareImage = _country.FlagSquareImage;

                    IsDirty = false;
                }
                else if (e.Error is FaultException<ExceptionDetail>)
                {
                    var serviceFault = e.Error as FaultException<ExceptionDetail>;
                    
                    ShowMessagebox.Raise(new Notification
                    {
                        Title = "FaultException. Error loading country.",
                        Content = serviceFault.Detail.Message
                    }, notification => { });
                }
                else if (e.Error is CommunicationException)
                {
                    ShowMessagebox.Raise(new Notification
                    {
                        Title = "Exception.",
                        Content = "Cannot reach the service."
                    }, notification => { });
                }
                else if (e.Error != null)
                {
                    ShowMessagebox.Raise(new Notification
                    {
                        Title = "Exception. Error loading country.",
                        Content = e.Error.Message
                    }, notification => { });
                }
            };
        }

        private void SaveCountry()
        {
            if (HasErrors)
                return;

            var client = new GeographyServiceClientWrapper();

            var imagesToProcess = new List<ImageFileOperationDto>();

            // Form list of images to upload.
            var imagesToUpload = FormFlagsToUpload();
            if (imagesToUpload.Count != 0)
                imagesToProcess.AddRange(imagesToUpload);
            // Form list of images to delete.
            var imagesToDelete = FormImagesToDelete();
            if (imagesToDelete.Count != 0)
                imagesToProcess.AddRange(imagesToDelete);
            // Form list of images to update.
            var imagesToUpdate = FormImagesToUpdate();
            if (imagesToUpdate.Count != 0)
                imagesToProcess.AddRange(imagesToUpdate);

            var countryDto = new CountryDto
            {
                ContinentId = _selectedContinent.ContinentId,
                CountryId = _country.CountryId,
                EnglishCountryName = _englishCountryName,
                EnglishCountryFullName = _englishCountryFullName,
                NationalCountryName = _nationalCountryName,
                NationalCountryFullName = _nationalCountryFullName,
                ISO = _iso,
                PreciseLocation = _preciseLocation,
                Alpha2 = _alpha2,
                Alpha3 = _alpha3
            };

            client.UpdateCountryAsync(countryDto, imagesToProcess.ToObservableCollection());
            client.UpdateCountryCompleted += delegate (object sender, AsyncCompletedEventArgs e)
            {
                if (e.Error == null)
                {
                    // Refresh flag images uri's.
                    _country.CreateFlagFullImage(BitmapCreateOptions.IgnoreImageCache, FlagType.Full);
                    _country.CreateFlagRoundImage(BitmapCreateOptions.IgnoreImageCache, FlagType.Round);
                    _country.CreateFlagSqaureImage(BitmapCreateOptions.IgnoreImageCache, FlagType.Square);

                    EventAggregator.GetEvent<CountryAddedEvent>().Publish(Country);
                    OnClose();
                }
                else if (e.Error is FaultException<ExceptionDetail>)
                {
                    var serviceFault = e.Error as FaultException<ExceptionDetail>;

                    var innerException = serviceFault.Detail.InnerException.InnerException.Message;
                    
                    ShowMessagebox.Raise(new Notification
                    {
                        Title = "FaultException. Error saving country.",
                        Content = serviceFault.Detail.Message + Environment.NewLine + innerException
                    }, notification => { });
                }
                else if (e.Error is CommunicationException)
                {
                    ShowMessagebox.Raise(new Notification
                    {
                        Title = "Exception.",
                        Content = "Cannot reach the service."
                    }, notification => { });
                }
                else if (e.Error != null)
                {
                    ShowMessagebox.Raise(new Notification
                    {
                        Title = "Exception. Error saving country.",
                        Content = e.Error.Message
                    }, notification => { });
                }
            };
        }

        #endregion


        #region INavigationAware

        bool INavigationAware.IsNavigationTarget(NavigationContext navigationContext)
        {
            string countryId = navigationContext.Parameters["countryId"];
            if (!string.IsNullOrWhiteSpace(countryId) && Country != null && Country.CountryId == int.Parse(countryId))
                return true;
            return false;
        }

        void INavigationAware.OnNavigatedTo(NavigationContext navigationContext)
        {
            _country = new CountryWithFlags();

            // Force validate this fields.
            EnglishCountryName = string.Empty;
            NationalCountryName = string.Empty;
            Alpha3 = string.Empty;


            // Load country based on ID passed in navigation parameters.
            string countryId = navigationContext.Parameters["countryId"];
            if (!string.IsNullOrWhiteSpace(countryId))
            {
                GetCountry(int.Parse(countryId));
            }
            else
            {
                // Generate empty flags.
                _country.CreateFlagFullImage(BitmapCreateOptions.IgnoreImageCache, FlagType.Empty);
                _country.CreateFlagRoundImage(BitmapCreateOptions.IgnoreImageCache, FlagType.Empty);
                _country.CreateFlagSqaureImage(BitmapCreateOptions.IgnoreImageCache, FlagType.Empty);

                // Show empty flags.
                FlagFullImage = _country.FlagFullImage;
                FlagRoundImage = _country.FlagRoundImage;
                FlagSquareImage = _country.FlagSquareImage;
            }

            // Load continents based on ID passed in navigation parameters.
            string continentId = navigationContext.Parameters["continentId"];
            if (!string.IsNullOrWhiteSpace(continentId))
            {
                GetContinents(int.Parse(continentId));
            }
        }

        void INavigationAware.OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        #endregion



        #region Private methods

        private IList<ImageFileOperationDto> FormFlagsToUpload()
        {
            var imageFileList = new List<ImageFileOperationDto>();

            if (_country.Alpha3 == null || _country.Alpha3.Trim() == string.Empty)
                return imageFileList;

            const string container = "flags";

            if (_flagFullImageBytes != null && _flagFullImageBytes.Length > 0)
            {
                imageFileList.Add(new ImageFileOperationDto
                {
                    ImageBytes = _flagFullImageBytes,
                    Container = container,
                    FileName = _country.Alpha3 + "_full.png",
                    FileOperation = FileOperation.Save
                });
            }

            if (_flagRoundImageBytes != null && _flagRoundImageBytes.Length > 0)
            {
                imageFileList.Add(new ImageFileOperationDto
                {
                    ImageBytes = _flagRoundImageBytes,
                    Container = container,
                    FileName = _country.Alpha3 + "_round.png",
                    FileOperation = FileOperation.Save
                });
            }

            if (_flagSquareImageBytes != null && _flagSquareImageBytes.Length > 0)
            {
                imageFileList.Add(new ImageFileOperationDto
                {
                    ImageBytes = _flagSquareImageBytes,
                    Container = container,
                    FileName = _country.Alpha3 + "_square.png",
                    FileOperation = FileOperation.Save
                });
            }

            return imageFileList;
        }

        private IList<ImageFileOperationDto> FormImagesToDelete()
        {
            var imageFileList = new List<ImageFileOperationDto>();

            if (_country.Alpha3 == null || _country.Alpha3.Trim() == string.Empty)
                return imageFileList;

            const string container = "flags";

            var alpha3 = _country.Alpha3;
            if (_country.Alpha3 != OldAlpha3)
            {
                alpha3 = OldAlpha3;
            }

            if ((_flagFullImage.UriSource.ToString().ToLower().Contains("empty")))
            {
                imageFileList.Add(new ImageFileOperationDto
                {
                    ImageBytes = null,
                    Container = container,
                    FileName = alpha3 + "_full.png",
                    FileOperation = FileOperation.Delete
                });
            }

            if (_flagRoundImage.UriSource.ToString().ToLower().Contains("empty"))
            {
                imageFileList.Add(new ImageFileOperationDto
                {
                    ImageBytes = null,
                    Container = container,
                    FileName = alpha3 + "_round.png",
                    FileOperation = FileOperation.Delete
                });
            }

            if (_flagSquareImage.UriSource.ToString().ToLower().Contains("empty"))
            {
                imageFileList.Add(new ImageFileOperationDto
                {
                    ImageBytes = null,
                    Container = container,
                    FileName = alpha3 + "_sqaure.png",
                    FileOperation = FileOperation.Delete
                });
            }

            return imageFileList;
        }

        private IList<ImageFileOperationDto> FormImagesToUpdate()
        {
            var imageFileList = new List<ImageFileOperationDto>();

            if (_country.Alpha3 == null || _country.Alpha3.Trim() == string.Empty)
                return imageFileList;

            const string container = "flags";

            if (_country.Alpha3 != OldAlpha3 && !_flagFullImage.UriSource.ToString().ToLower().Contains("empty"))
            {
                imageFileList.Add(new ImageFileOperationDto
                {
                    ImageBytes = null,
                    Container = container,
                    FileName = _country.Alpha3 + "_full.png",
                    OldFileName = OldAlpha3 + "_full.png",
                    FileOperation = FileOperation.Update
                });
            }

            if (_country.Alpha3 != OldAlpha3 && !_flagRoundImage.UriSource.ToString().ToLower().Contains("empty"))
            {
                imageFileList.Add(new ImageFileOperationDto
                {
                    ImageBytes = null,
                    Container = container,
                    FileName = _country.Alpha3 + "_round.png",
                    OldFileName = OldAlpha3 + "_round.png",
                    FileOperation = FileOperation.Update
                });
            }

            if (_country.Alpha3 != OldAlpha3 && !_flagSquareImage.UriSource.ToString().ToLower().Contains("empty"))
            {
                imageFileList.Add(new ImageFileOperationDto
                {
                    ImageBytes = null,
                    Container = container,
                    FileName = _country.Alpha3 + "_square.png",
                    OldFileName = OldAlpha3 + "_square.png",
                    FileOperation = FileOperation.Update
                });
            }

            return imageFileList;
        }

        #endregion
    }
}
