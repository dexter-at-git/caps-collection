using CapsCollection.Silverlight.Infrastructure.Models;
using CapsCollection.Silverlight.Infrastructure.ViewModels;
using CapsCollection.Silverlight.ServiceAgents.Proxies.Beer;
using CapsCollection.Silverlight.UI.Modules.Collection.Helpers;
using CapsCollection.Silverlight.UI.Modules.Collection.Validators;
using CapsCollection.Silverlight.UI.Modules.Collection.Views;
using FluentValidation;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Regions;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using System.Windows.Media.Imaging;
using CapsCollection.Silverlight.Infrastructure.Events;
using CapsCollection.Silverlight.ServiceAgents.Proxies.Geography;
using CapsCollection.Silverlight.ServiceAgents.ProxiesWrappers;
using CapsCollection.Silverlight.UI.Modules.Services.Interfaces;
using FileOperation = CapsCollection.Silverlight.ServiceAgents.Proxies.Beer.FileOperation;
using ImageFileOperationDto = CapsCollection.Silverlight.ServiceAgents.Proxies.Beer.ImageFileOperationDto;

namespace CapsCollection.Silverlight.UI.Modules.Collection.ViewModels
{
    public class BeerEditViewModel : ViewModelBase, IConfirmNavigationRequest
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
        public DelegateCommand ClearBottleImageCommand { get; private set; }
        public DelegateCommand ClearCapImageCommand { get; private set; }
        public DelegateCommand ClearLabelImageCommand { get; private set; }

        #endregion


        #region Properties and Members

        public InteractionRequest<Notification> ShowMessagebox { get; set; }

        private readonly BeerModelValidator _validator;
        private ObservableCollection<ImageFileOperationDto> ImageFiles { get; set; }

        public string Title
        {
            get { return _beer != null ? _beer.BeerName : "Empty"; }
        }

        bool _isDirty;
        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                _isDirty = value;
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        BeerDto _beer;
        public BeerDto Beer
        {
            get { return _beer; }
            set
            {
                if (_beer != value)
                {
                    if (_beer != null) _beer.PropertyChanged -= OnBeerChanged;
                    _beer = value;
                    if (_beer != null) _beer.PropertyChanged += OnBeerChanged;
                    RaisePropertyChanged(() => Beer);
                    RaisePropertyChanged(() => Title);
                }
            }
        }

        private string _beerName;
        public string BeerName
        {
            get { return _beerName; }
            set
            {
                if (_beerName != value)
                {
                    _beerName = value;
                    Beer.BeerName = value;
                    RaisePropertyChanged(() => BeerName);
                }

                ClearErrorFromProperty("BeerName");
                var validationResult = _validator.Validate(Beer, "BeerName");
                if (!validationResult.IsValid)
                    validationResult.Errors.ToList().ForEach(x => AddErrorForProperty(x.PropertyName, new ErrorInfo { ErrorMessage = x.ErrorMessage, IsWarning = false }));

                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        private string _beerType;
        public string BeerType
        {
            get { return _beerType; }
            set
            {
                if (_beerType != value)
                {
                    _beerType = value;
                    Beer.BeerType = value;
                    RaisePropertyChanged(() => BeerType);
                }

                ClearErrorFromProperty("BeerType");
                var validationResult = _validator.Validate(Beer, "BeerType");
                if (!validationResult.IsValid)
                    validationResult.Errors.ToList().ForEach(x => AddErrorForProperty(x.PropertyName, new ErrorInfo { ErrorMessage = x.ErrorMessage, IsWarning = false }));

                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        private decimal? _beerPrice;
        public decimal? BeerPrice
        {
            get { return _beerPrice; }
            set
            {
                if (_beerPrice != value)
                {
                    _beerPrice = value;
                    Beer.BeerPrice = value;
                    RaisePropertyChanged(() => BeerPrice);
                }

                ClearErrorFromProperty("BeerPrice");
                var validationResult = _validator.Validate(Beer, "BeerPrice");
                if (!validationResult.IsValid)
                    validationResult.Errors.ToList().ForEach(x => AddErrorForProperty(x.PropertyName, new ErrorInfo { ErrorMessage = x.ErrorMessage, IsWarning = false }));

                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        private DateTime? _beerYear;
        public DateTime? BeerYear
        {
            get { return _beerYear; }
            set
            {
                if (_beerYear != value)
                {
                    _beerYear = value;
                    Beer.BeerYear = value;
                    RaisePropertyChanged(() => BeerYear);
                }

                ClearErrorFromProperty("BeerYear");
                var validationResult = _validator.Validate(Beer, "BeerYear");
                if (!validationResult.IsValid)
                    validationResult.Errors.ToList().ForEach(x => AddErrorForProperty(x.PropertyName, new ErrorInfo { ErrorMessage = x.ErrorMessage, IsWarning = false }));

                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        private string _beerSite;
        public string BeerSite
        {
            get { return _beerSite; }
            set
            {
                if (_beerSite != value)
                {
                    _beerSite = value;
                    Beer.BeerSite = value;
                    RaisePropertyChanged(() => BeerSite);
                }

                ClearErrorFromProperty("BeerSite");
                var validationResult = _validator.Validate(Beer, "BeerSite");
                if (!validationResult.IsValid)
                    validationResult.Errors.ToList().ForEach(x => AddErrorForProperty(x.PropertyName, new ErrorInfo { ErrorMessage = x.ErrorMessage, IsWarning = false }));

                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        private string _beerComment;
        public string BeerComment
        {
            get { return _beerComment; }
            set
            {
                if (_beerComment != value)
                {
                    _beerComment = value;
                    Beer.BeerComment = value;
                    RaisePropertyChanged(() => BeerComment);
                }

                ClearErrorFromProperty("BeerComment");
                var validationResult = _validator.Validate(Beer, "BeerComment");
                if (!validationResult.IsValid)
                    validationResult.Errors.ToList().ForEach(x => AddErrorForProperty(x.PropertyName, new ErrorInfo { ErrorMessage = x.ErrorMessage, IsWarning = false }));

                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        private ObservableCollection<ContinentDto> _continents;
        public ObservableCollection<ContinentDto> Continents
        {
            get
            {
                // Enable/disable control in UI.
                RaisePropertyChanged(() => IsContinentListEnabled);

                return _continents;
            }
            set
            {
                _continents.Clear();
                _continents = value;
                RaisePropertyChanged(() => Continents);
            }
        }

        private ObservableCollection<CountryDto> _countries;
        public ObservableCollection<CountryDto> Countries
        {
            get
            {
                // Enable/disable control in UI.
                RaisePropertyChanged(() => IsCountryListEnabled);

                return _countries;
            }
            set
            {
                _countries.Clear();
                _countries = value;
                RaisePropertyChanged(() => Countries);
            }
        }

        private ObservableCollection<BreweryDto> _breweries;
        public ObservableCollection<BreweryDto> Breweries
        {
            get
            {
                // Enable/disable control in UI.
                RaisePropertyChanged(() => IsBreweryListEnabled);

                return _breweries;
            }
            set
            {
                _breweries.Clear();
                _breweries = value;
                RaisePropertyChanged(() => Breweries);
            }
        }

        private readonly ObservableCollection<BeerStyleDto> _beerStyles;
        public ObservableCollection<BeerStyleDto> BeerStyles
        {
            get
            {
                return _beerStyles;
            }
            set
            {
                _beerStyles.Clear();
                _beerStyles.Add(EmptyValues.GetEmptyBeerStyle());
                if (value != null)
                {
                    foreach (var beerStyleDto in value)
                    {
                        _beerStyles.Add(beerStyleDto);
                    }
                }

                // Choose default value.
                _selectedBeerStyle = _beerStyles.FirstOrDefault(c => c.BeerStyleId == -1);
                RaisePropertyChanged(() => SelectedBeerStyle);
            }
        }

        private readonly ObservableCollection<CapTypeDto> _capTypes;
        public ObservableCollection<CapTypeDto> CapTypes
        {
            get
            {
                return _capTypes;
            }
            set
            {
                _capTypes.Clear();
                _capTypes.Add(EmptyValues.GetEmptyCapType());
                if (value != null)
                {
                    foreach (var capTypeDto in value)
                    {
                        _capTypes.Add(capTypeDto);
                    }
                }

                // Choose default value.
                _selectedCapType = _capTypes.FirstOrDefault(c => c.CapTypeId == -1);
                RaisePropertyChanged(() => SelectedCapType);
            }
        }

        private ContinentDto _selectedContinent;
        public ContinentDto SelectedContinent
        {
            get
            {

                // Get countries from DB 
                if (_selectedContinent != null)
                {
                    GetContinentCountries(_selectedContinent.ContinentId);
                }

                return _selectedContinent;
            }
            set
            {
                if (_selectedContinent != value)
                {
                    _selectedContinent = value;
                    RaisePropertyChanged(() => SelectedContinent);
                }
            }
        }

        private CountryDto _selectedCountry;
        public CountryDto SelectedCountry
        {
            get
            {
                if (_selectedCountry != null)
                {
                    GetCountryBreweries(_selectedCountry.CountryId);
                }


                if (_selectedCountry != null)
                {
                    CountryId = _selectedCountry.CountryId;
                }

                return _selectedCountry;
            }
            set
            {
                _selectedCountry = value;
                RaisePropertyChanged(() => SelectedCountry);
            }
        }

        BreweryDto _selectedBrewery;
        public BreweryDto SelectedBrewery
        {
            get
            {
                if (_selectedBrewery != null)
                {
                    BreweryId = _selectedBrewery.BreweryId;
                }

                return _selectedBrewery;
            }
            set
            {
                _selectedBrewery = value;
                RaisePropertyChanged(() => SelectedBrewery);

                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        BeerStyleDto _selectedBeerStyle;
        public BeerStyleDto SelectedBeerStyle
        {
            get
            {
                return _selectedBeerStyle;
            }
            set
            {
                if (_selectedBeerStyle != value)
                {
                    _selectedBeerStyle = value;
                    BeerStyleId = _selectedBeerStyle.BeerStyleId;
                    RaisePropertyChanged(() => SelectedBeerStyle);
                }

                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        CapTypeDto _selectedCapType;
        public CapTypeDto SelectedCapType
        {
            get
            {
                return _selectedCapType;
            }
            set
            {
                if (_selectedCapType != value)
                {
                    _selectedCapType = value;
                    CapTypeId = _selectedCapType.CapTypeId;

                    RaisePropertyChanged(() => SelectedCapType);
                }

                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        private int _breweryId;
        public int BreweryId
        {
            get { return _breweryId; }
            set
            {
                if (_breweryId != value)
                {
                    _breweryId = value;
                    Beer.BreweryId = value;
                    RaisePropertyChanged(() => BreweryId);
                }

                ClearErrorFromProperty("SelectedBrewery");
                var validationResult = _validator.Validate(Beer, "BreweryId");
                if (!validationResult.IsValid)
                    validationResult.Errors.ToList().ForEach(x => AddErrorForProperty("SelectedBrewery", new ErrorInfo { ErrorMessage = x.ErrorMessage, IsWarning = false }));

                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        private int _countryId;
        public int CountryId
        {
            get { return _countryId; }
            set
            {
                if (_countryId != value)
                {
                    _countryId = value;
                    Beer.CountryId = value;
                    RaisePropertyChanged(() => CountryId);
                }

                ClearErrorFromProperty("SelectedCountry");
                var validationResult = _validator.Validate(Beer, "CountryId");
                if (!validationResult.IsValid)
                    validationResult.Errors.ToList().ForEach(x => AddErrorForProperty("SelectedCountry", new ErrorInfo { ErrorMessage = x.ErrorMessage, IsWarning = false }));

                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        private int _beerStyleId;
        public int BeerStyleId
        {
            get { return _beerStyleId; }
            set
            {
                if (_beerStyleId != value)
                {
                    _beerStyleId = value;
                    Beer.BeerStyleId = value;
                    RaisePropertyChanged(() => BeerStyleId);
                }

                ClearErrorFromProperty("SelectedBeerStyle");
                var validationResult = _validator.Validate(Beer, "BeerStyleId");
                if (!validationResult.IsValid)
                    validationResult.Errors.ToList().ForEach(x => AddErrorForProperty("SelectedBeerStyle", new ErrorInfo { ErrorMessage = x.ErrorMessage, IsWarning = false }));

                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        private int _capTypeId;
        public int CapTypeId
        {
            get { return _capTypeId; }
            set
            {
                if (_capTypeId != value)
                {
                    _capTypeId = value;
                    Beer.CapTypeId = value;
                    RaisePropertyChanged(() => CapTypeId);
                }

                ClearErrorFromProperty("SelectedCapType");
                var validationResult = _validator.Validate(Beer, "CapTypeId");
                if (!validationResult.IsValid)
                    validationResult.Errors.ToList().ForEach(x => AddErrorForProperty("SelectedCapType", new ErrorInfo { ErrorMessage = x.ErrorMessage, IsWarning = false }));

                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        private string _statusMessage;
        public string StatusMessage
        {
            get { return _statusMessage; }
            set
            {
                _statusMessage = value;
                RaisePropertyChanged(() => StatusMessage);
            }
        }

        #endregion


        #region Enable/disable controls properties

        public bool IsContinentListEnabled
        {
            get
            {
                return _continents.Count > 1;
            }
        }

        public bool IsCountryListEnabled
        {
            get
            {
                return !EmptyValues.IsContinentEmpty(_selectedContinent);
            }
        }

        public bool IsBreweryListEnabled
        {
            get
            {
                return !EmptyValues.IsCountryEmpty(_selectedCountry);
            }
        }

        #endregion


        #region Image properties

        private byte[] _selectedBottleImage;
        public byte[] SelectedBottleImage
        {
            get
            {
                return _selectedBottleImage;
            }
            set
            {
                if (_selectedBottleImage == value) return;
                _selectedBottleImage = value;

                if (value != null)
                {
                    using (var ms = new MemoryStream(value))
                    {
                        BottleImage.ThumbnailImage = new BitmapImage();
                        BottleImage.ThumbnailImage.SetSource(ms);
                    }
                    BottleImage.ImageHiResBytes = value;
                }

                RaisePropertyChanged(() => SelectedBottleImage);

                IsDirty = true;
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        private byte[] _selectedCapImage;
        public byte[] SelectedCapImage
        {
            get
            {
                return _selectedCapImage;
            }
            set
            {
                if (_selectedCapImage == value) return;
                _selectedCapImage = value;

                if (value != null)
                {
                    using (var ms = new MemoryStream(value))
                    {
                        CapImage.ThumbnailImage = new BitmapImage();
                        CapImage.ThumbnailImage.SetSource(ms);
                    }
                    CapImage.ImageHiResBytes = value;
                }

                RaisePropertyChanged(() => SelectedCapImage);

                IsDirty = true;
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        private byte[] _selectedLabelImage;
        public byte[] SelectedLabelImage
        {
            get
            {
                return _selectedLabelImage;
            }
            set
            {
                if (_selectedLabelImage == value) return;
                _selectedLabelImage = value;

                if (value != null)
                {
                    using (var ms = new MemoryStream(value))
                    {
                        LabelImage.ThumbnailImage = new BitmapImage();
                        LabelImage.ThumbnailImage.SetSource(ms);
                    }
                    LabelImage.ImageHiResBytes = value;
                }

                RaisePropertyChanged(() => SelectedLabelImage);

                IsDirty = true;
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        private BeerImage _bottleImage;
        public BeerImage BottleImage
        {
            get
            {
                return _bottleImage;
            }
            set
            {
                _bottleImage = value;
                IsDirty = true;
                RaisePropertyChanged(() => BottleImage);
            }
        }

        private BeerImage _capImage;
        public BeerImage CapImage
        {
            get
            {
                return _capImage;
            }
            set
            {
                _capImage = value;
                IsDirty = true;
                RaisePropertyChanged(() => CapImage);
            }
        }

        private BeerImage _labelImage;
        public BeerImage LabelImage
        {
            get
            {
                return _labelImage;
            }
            set
            {
                _labelImage = value;
                IsDirty = true;
                RaisePropertyChanged(() => LabelImage);
            }
        }

        #endregion


        #region Constructors

        public BeerEditViewModel(BeerModelValidator validator)
        {
            // Workaround for bug in Silverlight 5 when raise property changed event not firing in debug mode.
            // http://stackoverflow.com/questions/11965408/coerce-value-in-property-setter-silverlight-5
            System.Windows.Data.Binding.IsDebuggingEnabled = false;

            if (validator == null)
                throw new ArgumentNullException("validator");

            _validator = validator;

            // Main lists.
            _continents = new ObservableCollection<ContinentDto>();
            _countries = new ObservableCollection<CountryDto>();
            _breweries = new ObservableCollection<BreweryDto>();
            _beerStyles = new ObservableCollection<BeerStyleDto>();
            _capTypes = new ObservableCollection<CapTypeDto>();
            ImageFiles = new ObservableCollection<ImageFileOperationDto>();

            // Commands.
            SaveCommand = new DelegateCommand(OnSave, CanSave);
            CloseCommand = new DelegateCommand(OnClose);
            ClearBottleImageCommand = new DelegateCommand(OnClearBottleImage);
            ClearCapImageCommand = new DelegateCommand(OnClearCapImage);
            ClearLabelImageCommand = new DelegateCommand(OnClearLabelImage);

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

            SaveBeer();

            BeerWithImages beer = new BeerWithImages();
            beer.BeerId = _beer.BeerId;
            beer.BeerName = _beer.BeerName;

            if (!(_bottleImage.ThumbnailImage.UriSource.ToString().ToLower().Contains("empty")))
            {
                beer.BottleImage = _bottleImage;
            }

            if (!(_capImage.ThumbnailImage.UriSource.ToString().ToLower().Contains("empty")))
            {
                beer.CapImage = _capImage;
            }
            if (!(_labelImage.ThumbnailImage.UriSource.ToString().ToLower().Contains("empty")))
            {
                beer.LabelImage = _labelImage;
            }

            EventAggregator.GetEvent<BeerAddedEvent>().Publish(beer);
        }



        private void OnClose()
        {
            IRegion editRegionContentRegion = RegionManager.Regions["BeerEditContent"];
            foreach (var view in editRegionContentRegion.Views)
            {
                if (view is BeerEditView && ((FrameworkElement)view).DataContext == this)
                {
                    editRegionContentRegion.Remove(view);
                    break;
                }
            }

            // Hide edit beer region
            if (editRegionContentRegion.Views.Count() == 0)
            {
                EventAggregator.GetEvent<ShowBeerEditRegionEvent>().Publish(false);
            }

            IRegion addBreweryContentRegion = RegionManager.Regions["PopupRegionContent"];
            foreach (var view in addBreweryContentRegion.Views)
            {
                if (view is BeerEditView && ((FrameworkElement)view).DataContext == this)
                {
                    addBreweryContentRegion.Remove(view);
                    break;
                }
            }
        }

        private void OnBeerChanged(object sender, PropertyChangedEventArgs e)
        {
            IsDirty = true;
        }

        private void OnClearBottleImage()
        {
            SelectedBottleImage = null;

            // Clear bottle image.
            _bottleImage.ClearImage();
            IsDirty = true;
        }

        private void OnClearCapImage()
        {
            SelectedCapImage = null;

            // Clear bottle image.
            _capImage.ClearImage();
            IsDirty = true;
        }

        private void OnClearLabelImage()
        {
            SelectedLabelImage = null;

            // Clear bottle image.
            _labelImage.ClearImage();
            IsDirty = true;
        }

        #endregion


        #region Methods to communicate with database

        public void GetBeer(int beerId)
        {
            var client = new BeerServiceClientWrapper();

            client.GetBeerAsync(beerId);
            IsBusy = true;
            StatusMessage = "Loading beer from database ...";
            client.GetBeerCompleted += delegate (object sender, GetBeerCompletedEventArgs e)
            {
                IsBusy = false;
                if (e.Error == null)
                {
                    BeerDto beer = e.Result;

                    Beer = beer;

                    BeerName = e.Result.BeerName;
                    BeerType = e.Result.BeerType;
                    BeerPrice = e.Result.BeerPrice;
                    BeerYear = e.Result.BeerYear;
                    BeerSite = e.Result.BeerSite;
                    BeerComment = e.Result.BeerComment;
                    CapTypeId = e.Result.CapTypeId;

                    GetContinents();
                    GetCapTypes();
                    GetBeerStyles();

                    IsDirty = false;
                }
                else if (e.Error is FaultException<ExceptionDetail>)
                {
                    var serviceFault = e.Error as FaultException<ExceptionDetail>;

                    
                    ShowMessagebox.Raise(new Notification { Title = "FaultException. Error getting beer.", Content = serviceFault.Detail.Message });
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
                    
                    ShowMessagebox.Raise(new Notification { Title = "Exception. Error getting beer.", Content = e.Error.Message });
                }
            };
        }


        private void SaveBeer()
        {
            if (HasErrors)
                return;

            var client = new BeerServiceClientWrapper();

            client.UpdateBeerAsync(Beer);

            IsBusy = true;
            StatusMessage = "Saving beer in database ...";

            client.UpdateBeerCompleted += delegate (object sender, UpdateBeerCompletedEventArgs e)
            {
                IsBusy = false;
                if (e.Error == null)
                {
                    StatusMessage = "Beer saved in database.";

                    if (Beer.BeerId == 0)
                    {
                        EventAggregator.GetEvent<CountryBeerAddedEvent>().Publish(_beer.CountryId);
                    }

                    Beer.BeerId = e.Result;

                    // Form list with images to upload.
                    FormImagesToUpload(BottleImage, BeerImageType.Bottle, Beer.BeerId);
                    FormImagesToUpload(CapImage, BeerImageType.Cap, Beer.BeerId);
                    FormImagesToUpload(LabelImage, BeerImageType.Label, Beer.BeerId);

                    // Form images to delete.
                    FormImagesToDelete(BottleImage);
                    FormImagesToDelete(CapImage);
                    FormImagesToDelete(LabelImage);

                    // Save changes in file storage.
                    ProcessImageFiles(ImageFiles);
                }
                else if (e.Error is FaultException<ExceptionDetail>)
                {
                    var serviceFault = e.Error as FaultException<ExceptionDetail>;

                    var innerException = serviceFault.Detail.InnerException.InnerException.Message;
                    
                    ShowMessagebox.Raise(new Notification { Title = "FaultException. Error saving beer.", Content = serviceFault.Detail.Message + Environment.NewLine + innerException });
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
                    
                    ShowMessagebox.Raise(new Notification { Title = "Exception. Error saving beer.", Content = e.Error.Message });
                }
            };

        }

        private void ProcessImageFiles(ObservableCollection<ImageFileOperationDto> imageFiles)
        {
            var client = new BeerServiceClientWrapper();
            client.ProcessImageFilesAsync(imageFiles);

            IsBusy = true;
            StatusMessage = "Uploading beer images to file storage ...";

            client.ProcessImageFilesCompleted += delegate (object sender, AsyncCompletedEventArgs e)
            {
                IsBusy = false;

                if (e.Error == null)
                {
                }
                else if (e.Error is FaultException<ExceptionDetail>)
                {
                    var serviceFault = e.Error as FaultException<ExceptionDetail>;

                    var innerException = serviceFault.Detail.InnerException;
                    
                    ShowMessagebox.Raise(new Notification { Title = "FaultException. Error during file operations.", Content = serviceFault.Detail.Message + Environment.NewLine + innerException });
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
                    ShowMessagebox.Raise(new Notification { Title = "Exception. Error during file operations.", Content = e.Error.Message });
                }

                OnClose();
            };
        }

        public void GetContinents()
        {
            IsBusy = true;

            var client = new GeographyServiceClientWrapper();

            client.GetContinentsWithBreweriesAsync();

            IsBusy = true;
            StatusMessage = "Loading continents from database ...";
            client.GetContinentsWithBreweriesCompleted += delegate (object sender, GetContinentsWithBreweriesCompletedEventArgs e)
            {
                IsBusy = false;
                if (e.Error == null)
                {
                    ObservableCollection<ContinentDto> continents = e.Result;

                    // Add default empty continent and values from database.
                    _continents.Clear();
                    _continents.Add(EmptyValues.GetEmptyContinent());
                    if (continents != null)
                    {
                        _continents.AddRange(continents);
                    }
                    RaisePropertyChanged(() => Continents);

                    // If continentId is 0 or -1 then selected continent is default empty value.
                    if (_beer.ContinentId == 0 || _beer.ContinentId == -1)
                    {
                        _selectedContinent = _continents.FirstOrDefault(c => c.ContinentId == -1);
                    }
                    else
                    {
                        _selectedContinent = _continents.FirstOrDefault(c => c.ContinentId == _beer.ContinentId);
                    }
                    RaisePropertyChanged(() => SelectedContinent);
                }
                else if (e.Error is FaultException<ExceptionDetail>)
                {
                    var serviceFault = e.Error as FaultException<ExceptionDetail>;
                    
                    ShowMessagebox.Raise(new Notification { Title = "FaultException. Error loading continents.", Content = serviceFault.Detail.Message });
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
                    ShowMessagebox.Raise(new Notification { Title = "Exception. Error loading continents.", Content = e.Error.Message });
                }

                IsBusy = false;
            };
        }

        public void GetContinentCountries(int continentId)
        {
            IsBusy = true;

            var client = new GeographyServiceClientWrapper();

            client.GetContinentCountriesWithBreweriesAsync(continentId, false);

            IsBusy = true;
            StatusMessage = "Loading countries from database ...";
            client.GetContinentCountriesWithBreweriesCompleted += delegate (object sender, GetContinentCountriesWithBreweriesCompletedEventArgs e)
            {
                IsBusy = false;
                if (e.Error == null)
                {
                    ObservableCollection<CountryDto> countries = e.Result;

                    // If continent is not selected then add dummy record.
                    _countries.Clear();
                    if (EmptyValues.IsContinentEmpty(_selectedContinent))
                    {
                        _countries.Add(EmptyValues.GetEmptyCountry());
                    }
                    else
                    {
                        _countries.Add(EmptyValues.GetEmptyCountry(_selectedContinent));
                    }

                    if (countries != null)
                    {
                        _countries.AddRange(countries);
                    }
                    RaisePropertyChanged(() => Countries);

                    if (_beer.CountryId == 0 || _beer.CountryId == -1 || _beer.ContinentId != _selectedContinent.ContinentId)
                    {
                        _selectedCountry = _countries.FirstOrDefault(c => c.CountryId == -1);
                    }
                    else
                    {
                        _selectedCountry = _countries.FirstOrDefault(c => c.CountryId == _beer.CountryId);
                    }
                    RaisePropertyChanged(() => SelectedCountry);
                }
                else if (e.Error is FaultException<ExceptionDetail>)
                {
                    var serviceFault = e.Error as FaultException<ExceptionDetail>;
                    
                    ShowMessagebox.Raise(new Notification { Title = "FaultException. Error getting countries.", Content = serviceFault.Detail.Message });
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
                    ShowMessagebox.Raise(new Notification { Title = "Exception. Error getting countries.", Content = e.Error.Message });
                }

                IsBusy = false;
            };
        }

        public void GetCountryBreweries(int countryId)
        {
            IsBusy = true;

            var client = new BeerServiceClientWrapper();

            client.GetBreweriesByCountryAsync(countryId);

            IsBusy = true;
            StatusMessage = "Loading breweries from database ...";
            client.GetBreweriesByCountryCompleted += delegate (object sender, GetBreweriesByCountryCompletedEventArgs e)
            {
                IsBusy = false;
                if (e.Error == null)
                {
                    ObservableCollection<BreweryDto> breweries = e.Result;

                    // If continent is not selected then add dummy record.
                    _breweries.Clear();
                    if (EmptyValues.IsCountryEmpty(_selectedCountry))
                    {
                        _breweries.Add(EmptyValues.GetEmptyBrewery());
                    }
                    else
                    {
                        _breweries.Add(EmptyValues.GetEmptyBrewery(_selectedCountry));
                    }

                    if (breweries != null)
                    {
                        _breweries.AddRange(breweries);
                    }
                    RaisePropertyChanged(() => Breweries);

                    if (_beer.BreweryId == 0 || _beer.BreweryId == -1 || _beer.CountryId != _selectedCountry.CountryId)
                    {
                        _selectedBrewery = _breweries.FirstOrDefault(c => c.BreweryId == -1);
                    }
                    else
                    {
                        _selectedBrewery = _breweries.FirstOrDefault(c => c.BreweryId == _beer.BreweryId);
                    }
                    RaisePropertyChanged(() => SelectedBrewery);

                }
                else if (e.Error is FaultException<ExceptionDetail>)
                {
                    var serviceFault = e.Error as FaultException<ExceptionDetail>;
                    
                    ShowMessagebox.Raise(new Notification { Title = "FaultException. Error getting breweries.", Content = serviceFault.Detail.Message });
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
                    
                    ShowMessagebox.Raise(new Notification { Title = "Exception. Error getting breweries.", Content = e.Error.Message });
                }

                IsBusy = false;
            };

        }

        public void GetBeerStyles()
        {
            IsBusy = true;

            var client = new BeerServiceClientWrapper();

            client.GetBeerStylesAsync();

            IsBusy = true;
            StatusMessage = "Loading beer styles from database ...";
            client.GetBeerStylesCompleted += delegate (object sender, GetBeerStylesCompletedEventArgs e)
            {
                IsBusy = false;
                if (e.Error == null)
                {
                    ObservableCollection<BeerStyleDto> beerStyles = e.Result;

                    BeerStyles = beerStyles;

                    if (Beer.BeerStyleId > 0 && beerStyles != null)
                    {
                        SelectedBeerStyle = beerStyles.FirstOrDefault(c => c.BeerStyleId == Beer.BeerStyleId);
                    }
                }
                else if (e.Error is FaultException<ExceptionDetail>)
                {
                    var serviceFault = e.Error as FaultException<ExceptionDetail>;
                    
                    ShowMessagebox.Raise(new Notification { Title = "FaultException. Error loading beer styles.", Content = serviceFault.Detail.Message });
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
                    ShowMessagebox.Raise(new Notification { Title = "Exception. Error loading beer styles.", Content = e.Error.Message });
                }

                IsBusy = false;
            };
        }

        public void GetCapTypes()
        {
            IsBusy = true;

            var client = new BeerServiceClientWrapper();

            client.GetCapTypesAsync();

            IsBusy = true;
            StatusMessage = "Loading cap types from database ...";
            client.GetCapTypesCompleted += delegate (object sender, GetCapTypesCompletedEventArgs e)
            {
                IsBusy = false;
                if (e.Error == null)
                {
                    ObservableCollection<CapTypeDto> capTypes = e.Result;

                    CapTypes = capTypes;

                    if (Beer.CapTypeId > 0 && capTypes != null)
                    {
                        SelectedCapType = capTypes.FirstOrDefault(c => c.CapTypeId == Beer.CapTypeId);
                    }
                }
                else if (e.Error is FaultException<ExceptionDetail>)
                {
                    var serviceFault = e.Error as FaultException<ExceptionDetail>;
                    
                    ShowMessagebox.Raise(new Notification { Title = "FaultException. Error loading cap types.", Content = serviceFault.Detail.Message });
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
                    ShowMessagebox.Raise(new Notification { Title = "Exception. Error loading cap types.", Content = e.Error.Message });
                }

                IsBusy = false;
            };
        }

        #endregion


        #region INavigationAware

        bool INavigationAware.IsNavigationTarget(NavigationContext navigationContext)
        {
            string beerId = navigationContext.Parameters["beerId"];
            if (!string.IsNullOrWhiteSpace(beerId) && Beer != null && Beer.BeerId == int.Parse(beerId))
                return true;
            return false;
        }

        void INavigationAware.OnNavigatedTo(NavigationContext navigationContext)
        {
            Beer = new BeerDto();
            BeerName = null;
            BreweryId = -1;
            BeerStyleId = -1;
            CapTypeId = -1;

            // Initial load - Load beer based on ID passed in
            string beerId = navigationContext.Parameters["beerId"];

            if (string.IsNullOrWhiteSpace(beerId))
            {
                BottleImage = new BeerImage(BeerImageType.Bottle);
                CapImage = new BeerImage(BeerImageType.Cap);
                LabelImage = new BeerImage(BeerImageType.Label);

                GetContinents();
                GetBeerStyles();
                GetCapTypes();
            }
            else
            {
                BottleImage = new BeerImage(BeerImageType.Bottle, int.Parse(beerId));
                CapImage = new BeerImage(BeerImageType.Cap, int.Parse(beerId));
                LabelImage = new BeerImage(BeerImageType.Label, int.Parse(beerId));

                GetBeer(int.Parse(beerId));
            }

            _bottleImage.GetThumbnailImageWithFallback(BitmapCreateOptions.IgnoreImageCache);
            _capImage.GetThumbnailImageWithFallback(BitmapCreateOptions.IgnoreImageCache);
            _labelImage.GetThumbnailImageWithFallback(BitmapCreateOptions.IgnoreImageCache);
        }

        void INavigationAware.OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        #endregion


        #region IConfirmNavigationRequest

        void IConfirmNavigationRequest.ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
        {
            if (IsDirty)
            {
                const string prompt = "The view's state has changed and has not been saved, do you want to allow view switching?";
                var result = MessageBox.Show(prompt, "Confirmation", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    continuationCallback(true);
                    return;
                }

                continuationCallback(false);
                return;
            }
            continuationCallback(true);
        }

        #endregion


        #region Private methods

        private void FormImagesToUpload(BeerImage beerImage, BeerImageType imageType, int beerId)
        {
            // Empty uri means, that we must add new file images to our store.
            if (beerImage.ThumbnailImage.UriSource.ToString() != string.Empty)
                return;

            beerImage.CreateThumbnails(imageType, beerId);

            ImageFiles.Add(new ImageFileOperationDto
            {
                ImageBytes = beerImage.ImageHiResBytes,
                Container = beerImage.ImageContainerPath,
                FileName = beerImage.ImageHiResFileName,
                FileOperation = FileOperation.Save
            });

            ImageFiles.Add(new ImageFileOperationDto
            {
                ImageBytes = beerImage.ImagePreviewBytes,
                Container = beerImage.ImageContainerPath,
                FileName = beerImage.ImagePreviewFileName,
                FileOperation = FileOperation.Save
            });

            ImageFiles.Add(new ImageFileOperationDto
            {
                ImageBytes = beerImage.ImageThumbnailBytes,
                Container = beerImage.ImageContainerPath,
                FileName = beerImage.ImageThumbnailFileName,
                FileOperation = FileOperation.Save
            });
        }

        private void FormImagesToDelete(BeerImage beerImage)
        {
            // Uri with EmptyBottle.png or EmptyCap.png or EmptyLabel.png means, that we must delete those files from our storage.
            if (beerImage.ThumbnailImage.UriSource.ToString().ToLower().Contains("empty") && beerImage.BeerId != 0)
            {
                ImageFiles.Add(new ImageFileOperationDto
                {
                    ImageBytes = null,
                    Container = beerImage.ImageContainerPath,
                    FileName = beerImage.ImageHiResFileName,
                    FileOperation = FileOperation.Delete
                });

                ImageFiles.Add(new ImageFileOperationDto
                {
                    ImageBytes = null,
                    Container = beerImage.ImageContainerPath,
                    FileName = beerImage.ImagePreviewFileName,
                    FileOperation = FileOperation.Delete
                });

                ImageFiles.Add(new ImageFileOperationDto
                {
                    ImageBytes = null,
                    Container = beerImage.ImageContainerPath,
                    FileName = beerImage.ImageThumbnailFileName,
                    FileOperation = FileOperation.Delete
                });
            }
        }

        #endregion
    }
}
