using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using CapsCollection.Business.DTOs;
using CapsCollection.Desktop.Infrastructure.Commands;
using CapsCollection.Desktop.Infrastructure.Extensions;
using CapsCollection.Desktop.Infrastructure.Helpers;
using CapsCollection.Desktop.Infrastructure.Interfaces;
using CapsCollection.Desktop.Infrastructure.Models;
using CapsCollection.Desktop.UI.Modules.BulkUpdate.Views;
using System;
using System.Linq;
using Prism.Commands;
using Prism.Events;
using FluentValidation;

namespace CapsCollection.Desktop.UI.Modules.BulkUpdate.ViewModels
{
    public class BeerUpdateViewModel : ViewModelBase, IBeerUpdateViewModel
    {
        #region Private properties

        private readonly IValidator<BeerUpdateViewModel> _validator;
        private readonly IEventAggregator _eventAggregator;
        private readonly IValidatorFactory _validatorFactory;

        #endregion


        #region Commands and Events

        public DelegateCommand<BeerUpdateViewModel> SaveBeerCommand { get; private set; }

        #endregion


        #region Properties

        private ObservableCollection<BreweryDto> _allBreweries;
        public ObservableCollection<BreweryDto> AllBreweries
        {
            get { return _allBreweries; }
            set { base.SetProperty(ref _allBreweries, value); }
        }

        private Guid _beerTempId;
        public Guid BeerTempId
        {
            get { return _beerTempId; }
            set { base.SetProperty(ref _beerTempId, value); }
        }

        private int _beerId;
        public int BeerId
        {
            get { return _beerId; }
            set { base.SetProperty(ref _beerId, value); }
        }

        private string _beerName;
        public string BeerName
        {
            get { return _beerName; }
            set
            {
                base.SetProperty(ref _beerName, value);
                ValidateProperty();
                SaveBeerCommand.RaiseCanExecuteChanged();
            }
        }

        private string _beerType;
        public string BeerType
        {
            get { return _beerType; }
            set
            {
                base.SetProperty(ref _beerType, value);
                ValidateProperty();
                SaveBeerCommand.RaiseCanExecuteChanged();
            }
        }

        private decimal? _beerPrice;
        public decimal? BeerPrice
        {
            get { return _beerPrice; }
            set
            {
                base.SetProperty(ref _beerPrice, value);
                ValidateProperty();
                SaveBeerCommand.RaiseCanExecuteChanged();
            }
        }

        private DateTime? _beerYear;
        public DateTime? BeerYear
        {
            get { return _beerYear; }
            set
            {
                base.SetProperty(ref _beerYear, value);
                ValidateProperty();
                SaveBeerCommand.RaiseCanExecuteChanged();
            }
        }

        private string _beerSite;
        public string BeerSite
        {
            get { return _beerSite; }
            set
            {
                base.SetProperty(ref _beerSite, value);
                ValidateProperty();
                SaveBeerCommand.RaiseCanExecuteChanged();
            }
        }

        private string _beerComment;
        public string BeerComment
        {
            get { return _beerComment; }
            set
            {
                base.SetProperty(ref _beerComment, value);
                ValidateProperty();
                SaveBeerCommand.RaiseCanExecuteChanged();
            }
        }
        
        private ObservableCollection<BeerStyleDto> _beerStyles;
        public ObservableCollection<BeerStyleDto> BeerStyles
        {
            get { return _beerStyles; }
            set
            {
                base.SetProperty(ref _beerStyles, value);
                var emptyBeerStyle = EmptyValues.GetEmptyBeerStyle();
                _beerStyles.Add(emptyBeerStyle);
                _beerStyles.Move(_beerStyles.IndexOf(emptyBeerStyle), 0);

                // Set default value
                SelectedBeerStyle = _beerStyles.FirstOrDefault(c => c.BeerStyleId == -1);
            }
        }

        BeerStyleDto _selectedBeerStyle;
        public BeerStyleDto SelectedBeerStyle
        {
            get { return _selectedBeerStyle; }
            set
            {
                base.SetProperty(ref _selectedBeerStyle, value);
                ValidateProperty();
                SaveBeerCommand.RaiseCanExecuteChanged();
            }
        }
        
        private ObservableCollection<CapTypeDto> _capTypes;
        public ObservableCollection<CapTypeDto> CapTypes
        {
            get { return _capTypes; }
            set
            {
                base.SetProperty(ref _capTypes, value);
                var emptyCapType = EmptyValues.GetEmptyCapType();
                _capTypes.Add(emptyCapType);
                _capTypes.Move(_capTypes.IndexOf(emptyCapType), 0);

                // Set default value
                SelectedCapType = _capTypes.FirstOrDefault(c => c.CapTypeId == -1);
            }
        }

        CapTypeDto _selectedCapType;
        public CapTypeDto SelectedCapType
        {
            get { return _selectedCapType; }
            set
            {
                base.SetProperty(ref _selectedCapType, value);
                ValidateProperty();
                SaveBeerCommand.RaiseCanExecuteChanged();
            }
        }
        
        private ObservableCollection<CountryDto> _countries;
        public ObservableCollection<CountryDto> Countries
        {
            get { return _countries; }
            set
            {
                base.SetProperty(ref _countries, value);
                var emptyCountry = EmptyValues.GetEmptyCountry();
                _countries.Add(emptyCountry);
                _countries.Move(_countries.IndexOf(emptyCountry), 0);

                // Set default country
                SelectedCountry = _countries.FirstOrDefault(c => c.CountryId == -1);
            }
        }

        CountryDto _selectedCountry;
        public CountryDto SelectedCountry
        {
            get { return _selectedCountry; }
            set
            {
                base.SetProperty(ref _selectedCountry, value);
                Breweries = _allBreweries.Where(x => x.CountryId == _selectedCountry.CountryId).ToObservableCollection();

                // Enable/disable control in UI
                OnPropertyChanged(() => IsCountryListEnabled);
            }
        }
        
        private ObservableCollection<BreweryDto> _breweries;
        public ObservableCollection<BreweryDto> Breweries
        {
            get
            {
                // If country is not selected then add dummy record
                if (EmptyValues.IsCountryEmpty(_selectedCountry))
                {
                    _breweries.Clear();
                    _breweries.Add(EmptyValues.GetEmptyBrewery());
                }
                return _breweries;
            }
            set
            {
                base.SetProperty(ref _breweries, value);
                var emptyBrewery = EmptyValues.GetEmptyBrewery(_selectedCountry);
                _breweries.Add(emptyBrewery);
                _breweries.Move(_breweries.IndexOf(emptyBrewery), 0);

                // Select default brewery
                SelectedBrewery = _breweries.FirstOrDefault(c => c.BreweryId == -1);

                // Enable/disable control in UI
                OnPropertyChanged(() => IsBreweryListEnabled);
            }
        }

        BreweryDto _selectedBrewery;
        public BreweryDto SelectedBrewery
        {
            get
            {
                // Select default value
                if (_selectedCountry != null && _selectedCountry.CountryId == -1)
                {
                    _selectedBrewery = _breweries.FirstOrDefault(c => c.BreweryId == -1);
                }
                return _selectedBrewery;
            }
            set
            {
                base.SetProperty(ref _selectedBrewery, value);
                ValidateProperty();
                SaveBeerCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion


        #region Image properties

        private BeerImage _bottleImage;
        public BeerImage BottleImage
        {
            get { return _bottleImage; }
            set { base.SetProperty(ref _bottleImage, value); }
        }

        private BeerImage _capImage;
        public BeerImage CapImage
        {
            get { return _capImage; }
            set { base.SetProperty(ref _capImage, value); }
        }

        private BeerImage _labelImage;
        public BeerImage LabelImage
        {
            get { return _labelImage; }
            set { base.SetProperty(ref _labelImage, value); }
        }

        #endregion


        #region Enable/disable controls properties

        public bool IsCountryListEnabled
        {
            get { return _countries.Count > 1; }
        }

        public bool IsBreweryListEnabled
        {
            get { return !EmptyValues.IsCountryEmpty(_selectedCountry); }
        }

        #endregion


        #region Constructor

        public BeerUpdateViewModel(IBeerUpdateView view, IEventAggregator eventAggregator, IValidatorFactory validatorFactory)
            : base(view)
        {
            _eventAggregator = eventAggregator;
            _validatorFactory = validatorFactory;
            _validator = validatorFactory.GetValidator<BeerUpdateViewModel>();

            // Commands
            SaveBeerCommand = new DelegateCommand<BeerUpdateViewModel>(OnSave, CanSave);

            // Collections
            _beerStyles = new ObservableCollection<BeerStyleDto>();
            _capTypes = new ObservableCollection<CapTypeDto>();
            _countries = new ObservableCollection<CountryDto>();
            _breweries = new ObservableCollection<BreweryDto>();
            _allBreweries = new ObservableCollection<BreweryDto>();
        }

        #endregion


        #region Buttons click command methods

        private bool CanSave(BeerUpdateViewModel beerViewModel)
        {
            return !HasErrors;
        }

        private void OnSave(BeerUpdateViewModel beerViewModel)
        {
            var beerDto = new BeerDto()
            {
                BeerId = _beerId,
                BeerName = _beerName,
                BeerPrice = _beerPrice,
                BeerSite = _beerSite,
                BeerType = _beerType,
                BeerStyleId = _selectedBeerStyle.BeerStyleId,
                BeerYear = _beerYear,
                BeerComment = _beerComment,
                BreweryId = _selectedBrewery.BreweryId,
                CapTypeId = _selectedCapType.CapTypeId,
                CountryId = _selectedCountry.CountryId,
                DateAdded = DateTime.Now
            };

            var beerToSave = new BeerSavingDataEventArgs()
            {
                Beer = beerDto,
                BottleImage = _bottleImage,
                CapImage = _capImage,
                LabelImage = _labelImage,
                BeerTempId = beerViewModel.BeerTempId
            };

            // Send beer to update
            _eventAggregator.GetEvent<BeerSavingEvent>().Publish(beerToSave);
        }

        #endregion


        #region Methods

        public BeerUpdateViewModel PrepareViewModel(BeerLoadDataEventArgs imageList)
        {
            var bottleImage = imageList.ImageList.FirstOrDefault(x => x.ImageType == ImageType.Bottle);
            var capImage = imageList.ImageList.FirstOrDefault(x => x.ImageType == ImageType.Cap);
            var labelImage = imageList.ImageList.FirstOrDefault(x => x.ImageType == ImageType.Label);

            var beerLoadViewModel = new BeerUpdateViewModel(new BeerUpdateView(), _eventAggregator, _validatorFactory);
            beerLoadViewModel.BeerTempId = Guid.NewGuid();
            beerLoadViewModel.BeerName = String.Empty;
            beerLoadViewModel.BeerStyles = imageList.BeerStyles.ToObservableCollection();
            beerLoadViewModel.CapTypes = imageList.CapTypes.ToObservableCollection();
            beerLoadViewModel.Countries = imageList.Countries.ToObservableCollection();
            beerLoadViewModel.Breweries = imageList.Breweries.ToObservableCollection();
            beerLoadViewModel.AllBreweries = imageList.Breweries.ToObservableCollection();

            int beerIndex = 0;
            if (imageList.Existing != null)
            {
                beerIndex = imageList.Existing.BeerId;
                beerLoadViewModel.BeerId = imageList.Existing.BeerId;
                beerLoadViewModel.BeerName = imageList.Existing.BeerName;
                beerLoadViewModel.BeerComment = imageList.Existing.BeerComment;
                beerLoadViewModel.BeerPrice = imageList.Existing.BeerPrice;
                beerLoadViewModel.BeerSite = imageList.Existing.BeerSite;
                beerLoadViewModel.BeerType = imageList.Existing.BeerType;
                beerLoadViewModel.BeerYear = imageList.Existing.BeerYear;
                beerLoadViewModel.SelectedCountry = imageList.Countries.FirstOrDefault(x => x.CountryId == imageList.Existing.CountryId);
                beerLoadViewModel.SelectedBrewery = imageList.Breweries.FirstOrDefault(x => x.BreweryId == imageList.Existing.BreweryId);
                beerLoadViewModel.SelectedCapType = imageList.CapTypes.FirstOrDefault(x => x.CapTypeId == imageList.Existing.CapTypeId);
                beerLoadViewModel.SelectedBeerStyle = imageList.BeerStyles.FirstOrDefault(x => x.BeerStyleId == imageList.Existing.BeerStyleId);
            }

            beerLoadViewModel.BottleImage = BeerImageBuilder.CreateBuilder(ImageType.Bottle).AttachThumbnails(bottleImage).AttachUri(beerIndex);
            beerLoadViewModel.CapImage = BeerImageBuilder.CreateBuilder(ImageType.Cap).AttachThumbnails(capImage).AttachUri(beerIndex);
            beerLoadViewModel.LabelImage = BeerImageBuilder.CreateBuilder(ImageType.Label).AttachThumbnails(labelImage).AttachUri(beerIndex);

            return beerLoadViewModel;
        }

        private void ValidateProperty([CallerMemberName]string propertyName = "")
        {
            ClearErrorFromProperty(propertyName);
            var validationResult = _validator.Validate(this, propertyName);
            if (!validationResult.IsValid)
            {
                validationResult.Errors.ToList().ForEach(x => AddErrorForProperty(x.PropertyName, new ErrorInfo { ErrorMessage = x.ErrorMessage, IsWarning = false }));
            }
        }

        #endregion


        #region Equals override

        public override bool Equals(object obj)
        {
            BeerUpdateViewModel viewModel = obj as BeerUpdateViewModel;
            if (viewModel == null)
            {
                return false;
            }

            return Equals(_bottleImage, viewModel._bottleImage) &&
                   Equals(_capImage, viewModel._capImage) &&
                   Equals(_labelImage, viewModel._labelImage);
        }

        public override int GetHashCode()
        {
            int hashCode = (_bottleImage != null ? _bottleImage.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (_capImage != null ? _capImage.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (_labelImage != null ? _labelImage.GetHashCode() : 0);
            return hashCode;
        }

        #endregion
    }
}
