using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using CapsCollection.Silverlight.Infrastructure.Models;
using CapsCollection.Silverlight.Infrastructure.ViewModels;
using CapsCollection.Silverlight.ServiceAgents.Proxies.Beer;
using CapsCollection.Silverlight.Infrastructure.Commands;
using CapsCollection.Silverlight.Infrastructure.Events;
using CapsCollection.Silverlight.ServiceAgents.Proxies.Geography;
using CapsCollection.Silverlight.ServiceAgents.ProxiesWrappers;
using CapsCollection.Silverlight.UI.Modules.Brewery.Helpers;
using CapsCollection.Silverlight.UI.Modules.Brewery.Validators;
using CapsCollection.Silverlight.UI.Modules.Brewery.Views;
using CapsCollection.Silverlight.UI.Modules.Services.Interfaces;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Regions;
using FluentValidation;

namespace CapsCollection.Silverlight.UI.Modules.Brewery.ViewModels
{
    public class BreweryEditViewModel : ViewModelBase, INavigationAware
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

        #endregion


        #region Properties and Members

        private readonly BreweryModelValidator _validator;

        public InteractionRequest<Notification> ShowMessagebox { get; set; }

        public string Title
        {
            get { return _editingBrewery != null ? _editingBrewery.Brewery : "Empty"; }
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

        BreweryDto _editingBrewery;
        public BreweryDto EditingBrewery
        {
            get { return _editingBrewery; }
            set
            {
                if (_editingBrewery != value)
                {
                    if (_editingBrewery != null) _editingBrewery.PropertyChanged -= OnBreweryChanged;
                    _editingBrewery = value;
                    if (_editingBrewery != null) _editingBrewery.PropertyChanged += OnBreweryChanged;
                    RaisePropertyChanged(() => EditingBrewery);
                    RaisePropertyChanged(() => Title);
                }
            }
        }

        private int _cityId;
        public int CityId
        {
            get { return _cityId; }
            set
            {
                if (_cityId != value)
                {
                    _cityId = value;
                    EditingBrewery.CityId = value;
                    RaisePropertyChanged(() => CityId);
                }

                ClearErrorFromProperty("SelectedCity");
                var validationResult = _validator.Validate(EditingBrewery, "CityId");
                if (!validationResult.IsValid)
                    validationResult.Errors.ToList().ForEach(x => AddErrorForProperty("SelectedCity", new ErrorInfo { ErrorMessage = x.ErrorMessage, IsWarning = false }));

                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        private string _brewery;
        public string Brewery
        {
            get { return _brewery; }
            set
            {
                if (_brewery != value)
                {
                    _brewery = value;
                    EditingBrewery.Brewery = value;
                    RaisePropertyChanged(() => Brewery);
                }

                ClearErrorFromProperty("Brewery");
                var validationResult = _validator.Validate(EditingBrewery, "Brewery");
                if (!validationResult.IsValid)
                    validationResult.Errors.ToList().ForEach(x => AddErrorForProperty(x.PropertyName, new ErrorInfo { ErrorMessage = x.ErrorMessage, IsWarning = false }));

                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        private string _site;
        public string Site
        {
            get { return _site; }
            set
            {
                if (_site != value)
                {
                    _site = value;
                    EditingBrewery.Site = value;
                    RaisePropertyChanged(() => Site);
                }

                ClearErrorFromProperty("Site");
                var validationResult = _validator.Validate(EditingBrewery, "Site");
                if (!validationResult.IsValid)
                    validationResult.Errors.ToList().ForEach(x => AddErrorForProperty(x.PropertyName, new ErrorInfo { ErrorMessage = x.ErrorMessage, IsWarning = false }));

                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        private string _comment;
        public string Comment
        {
            get { return _comment; }
            set
            {
                if (_comment != value)
                {
                    _comment = value;
                    EditingBrewery.Comment = value;
                    RaisePropertyChanged(() => Comment);
                }

                ClearErrorFromProperty("Comment");
                var validationResult = _validator.Validate(EditingBrewery, "Comment");
                if (!validationResult.IsValid)
                    validationResult.Errors.ToList().ForEach(x => AddErrorForProperty(x.PropertyName, new ErrorInfo { ErrorMessage = x.ErrorMessage, IsWarning = false }));

                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        private readonly ObservableCollection<ContinentDto> _continents;
        public ObservableCollection<ContinentDto> Continents
        {
            get
            {
                return _continents;
            }
            set
            {
                _continents.Clear();
                _continents.Add(EmptyValues.GetEmptyContinent());
                if (value != null)
                {
                    foreach (var continentDto in value)
                    {
                        _continents.Add(continentDto);
                    }
                }

                // Choose default value.
                if (_breweryFilter.ContinentId != -1)
                {
                    SelectedContinent = _continents.FirstOrDefault(c => c.ContinentId == BreweryFilter.ContinentId);
                }

                if (_editingBrewery.ContinentId == -1 && _breweryFilter.ContinentId == -1)
                {
                    SelectedContinent = _continents.FirstOrDefault(c => c.ContinentId == -1);
                }

                // Enable/disable control in UI.
                RaisePropertyChanged(() => IsContinentListEnabled);
            }
        }

        private ObservableCollection<CountryDto> _countries;
        public ObservableCollection<CountryDto> Countries
        {
            get
            {
                // If continent is not selected then add dummy record.
                if (EmptyValues.IsContinentEmpty(_selectedContinent))
                {
                    _countries.Clear();
                    _countries.Add(EmptyValues.GetEmptyCountry());
                }

                return _countries;
            }
            set
            {
                _countries.Clear();
                _countries.Add(EmptyValues.GetEmptyCountry(_selectedContinent));
                if (value != null)
                {
                    foreach (var countryDto in value)
                    {
                        _countries.Add(countryDto);
                    }
                }

                // Select default country.
                if (_breweryFilter.CountryId != -1)
                {
                    SelectedCountry = _countries.FirstOrDefault(c => c.CountryId == BreweryFilter.CountryId);
                }

                if (_editingBrewery.CountryId == -1 && _breweryFilter.CountryId == -1)
                {
                    SelectedCountry = _countries.FirstOrDefault(c => c.CountryId == -1);
                }
                // Enable/disable control in UI.
                RaisePropertyChanged(() => IsCountryListEnabled);
            }
        }

        private ObservableCollection<RegionDto> _regions;
        public ObservableCollection<RegionDto> Regions
        {
            get
            {
                // If country is not selected then add dummy record.
                if (EmptyValues.IsCountryEmpty(_selectedCountry))
                {
                    _regions.Clear();
                    _regions.Add(EmptyValues.GetEmptyRegion());
                }

                return _regions;
            }
            set
            {
                _regions.Clear();
                _regions.Add(EmptyValues.GetEmptyRegion(_selectedCountry));
                if (value != null)
                {
                    foreach (var regionDto in value)
                    {
                        _regions.Add(regionDto);
                    }
                }

                // Select default region.
                if (_breweryFilter.RegionId != -1)
                {
                    SelectedRegion = _regions.FirstOrDefault(c => c.RegionId == BreweryFilter.RegionId);
                }

                if (_editingBrewery.RegionId == -1 && _breweryFilter.RegionId == -1)
                {
                    SelectedRegion = _regions.FirstOrDefault(c => c.RegionId == -1);
                }
                // Enable/disable control in UI.
                RaisePropertyChanged(() => IsRegionListEnabled);
            }
        }

        private ObservableCollection<CityDto> _cities;
        public ObservableCollection<CityDto> Cities
        {
            get
            {
                // If region is not selected then add dummy record.
                if (EmptyValues.IsRegionEmpty(_selectedRegion))
                {
                    _cities.Clear();
                    _cities.Add(EmptyValues.GetEmptyCity());
                }

                return _cities;
            }
            set
            {
                _cities.Clear();
                _cities.Add(EmptyValues.GetEmptyCity(_selectedRegion));
                if (value != null)
                {
                    foreach (var cityDto in value)
                    {
                        _cities.Add(cityDto);
                    }
                }

                // Select default city.
                if (_breweryFilter.CityId != -1)
                {
                    SelectedCity = _cities.FirstOrDefault(c => c.CityId == BreweryFilter.CityId);
                }

                if (_editingBrewery.CityId == -1 && _breweryFilter.CityId == -1)
                {
                    SelectedCity = _cities.FirstOrDefault(c => c.CityId == -1);
                }
                // Enable/disable control in UI.
                RaisePropertyChanged(() => IsCityListEnabled);
            }
        }

        ContinentDto _selectedContinent;
        public ContinentDto SelectedContinent
        {
            get
            {
                return _selectedContinent;
            }
            set
            {
                if (_selectedContinent != value)
                {
                    _selectedContinent = value;
                    RaisePropertyChanged(() => SelectedContinent);
                }

                // Get countries from DB 
                if (_selectedContinent != null)
                {
                    GetContinentCountries(_selectedContinent.ContinentId);
                }
            }
        }

        CountryDto _selectedCountry;
        public CountryDto SelectedCountry
        {
            get
            {
                // Select default value.
                if (_selectedContinent != null && _selectedContinent.ContinentId == -1)
                {
                    _selectedCountry = _countries.FirstOrDefault(c => c.CountryId == -1);
                }
                return _selectedCountry;
            }
            set
            {
                if (_selectedCountry != value)
                {
                    _selectedCountry = value;
                    RaisePropertyChanged(() => SelectedCountry);
                }

                // Get regions from DB 
                if (_selectedCountry != null)
                {
                    GetCountryRegions(_selectedCountry.CountryId);
                }
            }
        }

        RegionDto _selectedRegion;
        public RegionDto SelectedRegion
        {
            get
            {
                // Select default value.
                if (_selectedCountry != null && _selectedCountry.CountryId == -1)
                {
                    _selectedRegion = _regions.FirstOrDefault(c => c.RegionId == -1);
                }
                return _selectedRegion;
            }
            set
            {
                if (_selectedRegion != value)
                {
                    _selectedRegion = value;
                    RaisePropertyChanged(() => SelectedRegion);
                }

                // Get regions from DB 
                if (_selectedRegion != null)
                {
                    GetRegionCities(_selectedRegion.RegionId);
                }
            }
        }

        CityDto _selectedCity;
        public CityDto SelectedCity
        {
            get { return _selectedCity; }
            set
            {
                if (_selectedCity != value)
                {
                    _selectedCity = value;
                    RaisePropertyChanged(() => SelectedCity);
                }

                if (_selectedCity != null)
                {
                    CityId = SelectedCity.CityId;
                }

                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        BreweryFilter _breweryFilter;
        BreweryFilter BreweryFilter
        {
            get { return _breweryFilter; }
            set
            {
                _breweryFilter = value;

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

        public bool IsRegionListEnabled
        {
            get
            {
                return !EmptyValues.IsCountryEmpty(_selectedCountry);
            }
        }

        public bool IsCityListEnabled
        {
            get
            {
                return !EmptyValues.IsRegionEmpty(_selectedRegion);
            }
        }

        #endregion


        #region Constructors

        public BreweryEditViewModel(BreweryModelValidator validator)
        {
            if (validator == null)
                throw new ArgumentNullException("validator");

            _validator = validator;

            _continents = new ObservableCollection<ContinentDto>();
            _countries = new ObservableCollection<CountryDto>();
            _regions = new ObservableCollection<RegionDto>();
            _cities = new ObservableCollection<CityDto>();
            _breweryFilter = new BreweryFilter();

            // Commands.
            SaveCommand = new DelegateCommand(OnSave, CanSave);
            CloseCommand = new DelegateCommand(OnClose);

            // Poopup message boxes.
            ShowMessagebox = new InteractionRequest<Notification>();

            if (!DesignerProperties.IsInDesignTool)
            {
                CompositionInitializer.SatisfyImports(this);
            }

            Commands.SaveAllCommand.RegisterCommand(SaveCommand);

            GetContinents();

            var breweryFilterRegionContext = RegionManager.Regions["BreweryFilterContent"].Context;
            if (breweryFilterRegionContext != null)
            {
                _breweryFilter = (BreweryFilter)breweryFilterRegionContext;
            }
        }

        #endregion


        #region Events methods

        private void OnSave()
        {
            IsDirty = false;
            SaveBrewery();
        }

        private bool CanSave()
        {
            IsAuthenticated = AuthenticationManager.AuthenticationInfo.IsAuthenticated;

            return IsDirty && !HasErrors && IsAuthenticated;
        }

        private void OnClose()
        {
            IRegion editBreweryContentRegion = RegionManager.Regions["EditBreweryContent"];
            foreach (var view in editBreweryContentRegion.Views)
            {
                if (view is BreweryEditView && ((FrameworkElement)view).DataContext == this)
                {
                    editBreweryContentRegion.Remove(view);
                    Commands.SaveAllCommand.UnregisterCommand(SaveCommand);
                    break;
                }
            }

            IRegion addBreweryContentRegion = RegionManager.Regions["PopupRegionContent"];
            foreach (var view in addBreweryContentRegion.Views)
            {
                if (view is BreweryEditView && ((FrameworkElement)view).DataContext == this)
                {
                    addBreweryContentRegion.Remove(view);
                    Commands.SaveAllCommand.UnregisterCommand(SaveCommand);
                    break;
                }
            }
        }

        private void OnBreweryChanged(object sender, PropertyChangedEventArgs e)
        {
            IsDirty = true;
        }

        #endregion


        #region Methods to communicate with database

        public void GetBrewery(int breweryId)
        {
            var client = new BeerServiceClientWrapper();

            client.GetBreweryAsync(breweryId);
            client.GetBreweryCompleted += delegate (object sender, GetBreweryCompletedEventArgs e)
            {
                if (e.Error == null)
                {
                    BreweryDto brewery = e.Result;

                    EditingBrewery = brewery;

                    Brewery = e.Result.Brewery;
                    Site = e.Result.Site;
                    Comment = e.Result.Comment;

                    if (brewery.ContinentId > 0)
                    {
                        SelectedContinent = _continents.First(c => c.ContinentId == brewery.ContinentId);
                    }

                    IsDirty = false;
                }
                else if (e.Error is FaultException<ExceptionDetail>)
                {
                    var serviceFault = e.Error as FaultException<ExceptionDetail>;
                    
                    ShowMessagebox.Raise(new Notification
                    {
                        Title = "FaultException. Error loading brewery.",
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
                        Title = "Exception. Error loading brewery.",
                        Content = e.Error.Message
                    }, notification => { });
                }
            };
        }

        private void SaveBrewery()
        {
            if (HasErrors)
                return;

            var client = new BeerServiceClientWrapper();

            client.UpdateBreweryAsync(EditingBrewery);
            client.UpdateBreweryCompleted += delegate (object sender, AsyncCompletedEventArgs e)
            {
                if (e.Error == null)
                {
                    EventAggregator.GetEvent<BreweryAddedEvent>().Publish(EditingBrewery);
                    OnClose();
                }
                else if (e.Error is FaultException<ExceptionDetail>)
                {
                    var serviceFault = e.Error as FaultException<ExceptionDetail>;

                    var innerException = serviceFault.Detail.InnerException.InnerException.Message;
                    
                    ShowMessagebox.Raise(new Notification
                    {
                        Title = "FaultException. Error saving brewery.",
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
                        Title = "Exception. Error saving brewery.",
                        Content = e.Error.Message
                    }, notification => { });
                }
            };
        }

        public void GetContinents()
        {
            IsBusy = true;

            var client = new GeographyServiceClientWrapper();

            client.GetContinentsAsync();
            client.GetContinentsCompleted += delegate (object sender, GetContinentsCompletedEventArgs e)
            {
                if (e.Error == null)
                {
                    ObservableCollection<ContinentDto> continents = e.Result;

                    Continents = continents;

                    if (_editingBrewery.BreweryId > 0)
                    {
                        GetBrewery(EditingBrewery.BreweryId);
                    }
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

                IsBusy = false;
            };
        }

        public void GetContinentCountries(int continentId)
        {
            IsBusy = true;

            var client = new GeographyServiceClientWrapper();

            client.GetCountriesByContinentAsync(continentId, false);
            client.GetCountriesByContinentCompleted += delegate (object sender, GetCountriesByContinentCompletedEventArgs e)
            {
                if (e.Error == null)
                {
                    ObservableCollection<CountryDto> listCountries = e.Result;

                    Countries = listCountries;

                    if (EditingBrewery.CountryId > 0)
                    {
                        SelectedCountry = _countries.FirstOrDefault(c => c.CountryId == EditingBrewery.CountryId);
                    }
                }
                else if (e.Error is FaultException<ExceptionDetail>)
                {
                    var serviceFault = e.Error as FaultException<ExceptionDetail>;
                    
                    ShowMessagebox.Raise(new Notification
                    {
                        Title = "FaultException. Error getting countries.",
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
                        Title = "Exception. Error getting countries.",
                        Content = e.Error.Message
                    }, notification => { });
                }

                IsBusy = false;
            };
        }

        private void GetCountryRegions(int countryId)
        {
            IsBusy = true;

            var client = new GeographyServiceClientWrapper();

            client.GetRegionsByCountryAsync(countryId);
            client.GetRegionsByCountryCompleted += delegate (object sender, GetRegionsByCountryCompletedEventArgs e)
            {
                if (e.Error == null)
                {
                    ObservableCollection<RegionDto> listRegions = e.Result;

                    Regions = listRegions;
                    
                    if (EditingBrewery.RegionId > 0)
                    {
                        SelectedRegion = _regions.FirstOrDefault(c => c.RegionId == EditingBrewery.RegionId);
                    }
                }
                else if (e.Error is FaultException<ExceptionDetail>)
                {
                    var serviceFault = e.Error as FaultException<ExceptionDetail>;
                    
                    ShowMessagebox.Raise(new Notification
                    {
                        Title = "FaultException. Error getting regions.",
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
                        Title = "Exception. Error getting regions.",
                        Content = e.Error.Message
                    }, notification => { });
                }

                IsBusy = false;
            };
        }

        private void GetRegionCities(int regionId)
        {
            IsBusy = true;

            var client = new GeographyServiceClientWrapper();

            client.GetCitiesByRegionAsync(regionId);
            client.GetCitiesByRegionCompleted += delegate (object sender, GetCitiesByRegionCompletedEventArgs e)
            {
                if (e.Error == null)
                {
                    ObservableCollection<CityDto> listCities = e.Result;

                    Cities = listCities;

                    if (EditingBrewery.CityId > 0)
                    {
                        SelectedCity = _cities.FirstOrDefault(c => c.CityId == EditingBrewery.CityId);
                    }
                }
                else if (e.Error is FaultException<ExceptionDetail>)
                {
                    var serviceFault = e.Error as FaultException<ExceptionDetail>;
                    
                    ShowMessagebox.Raise(new Notification
                    {
                        Title = "FaultException. Error getting city.",
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
                        Title = "Exception. Error getting city.",
                        Content = e.Error.Message
                    }, notification => { });
                }

                IsBusy = false;
            };
        }

        #endregion


        #region INavigationAware

        bool INavigationAware.IsNavigationTarget(NavigationContext navigationContext)
        {
            string brewId = navigationContext.Parameters["breweryId"];
            if (!string.IsNullOrWhiteSpace(brewId) && _editingBrewery != null && _editingBrewery.BreweryId == int.Parse(brewId))
                return true;
            return false;
        }

        void INavigationAware.OnNavigatedTo(NavigationContext navigationContext)
        {
            // Choosing to not refresh the customer every time the view is loaded
            if (_editingBrewery != null)
                return;

            _editingBrewery = new BreweryDto();
            _editingBrewery.ContinentId = -1;
            _editingBrewery.CountryId = -1;
            _editingBrewery.RegionId = -1;
            _editingBrewery.CityId = -1;

            Brewery = null;
            Site = null;
            Comment = null;
            IsDirty = true;

            // Initial load - Load customer based on ID passed in
            string brewId = navigationContext.Parameters["breweryId"];
            if (!string.IsNullOrWhiteSpace(brewId))
            {
                _editingBrewery.BreweryId = int.Parse(brewId);
            }
        }

        void INavigationAware.OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        #endregion
    }
}
