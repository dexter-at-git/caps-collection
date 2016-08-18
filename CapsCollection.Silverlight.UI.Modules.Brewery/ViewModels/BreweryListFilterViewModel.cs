using CapsCollection.Silverlight.Infrastructure.ViewModels;
using CapsCollection.Silverlight.UI.Modules.Brewery.Helpers;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Regions;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.ServiceModel;
using CapsCollection.Silverlight.Infrastructure.Events;
using CapsCollection.Silverlight.Infrastructure.Models;
using CapsCollection.Silverlight.ServiceAgents.Proxies.Geography;
using CapsCollection.Silverlight.ServiceAgents.ProxiesWrappers;

namespace CapsCollection.Silverlight.UI.Modules.Brewery.ViewModels
{
    public class BreweryListFilterViewModel : ViewModelBase
    {
        #region MEF imports

        [Import]
        public IRegionManager RegionManager { get; set; }
        [Import]
        public IEventAggregator EventAggregator { get; set; }

        #endregion


        #region Commands and Events

        public DelegateCommand FilterCommand { get; private set; }

        #endregion


        #region Properties and Members

        public InteractionRequest<Notification> ShowMessagebox { get; set; }

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
                SelectedContinent = _continents.FirstOrDefault(c => c.ContinentId == -1);

                // Enable/disable control in UI.
                RaisePropertyChanged(() => IsContinentListEnabled);
                FilterCommand.RaiseCanExecuteChanged();
            }
        }

        private readonly ObservableCollection<CountryDto> _countries;
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
                RaisePropertyChanged(() => Countries);

                // Select default country.
                SelectedCountry = _countries.FirstOrDefault(c => c.CountryId == -1);

                // Enable/disable control in UI.
                RaisePropertyChanged(() => IsCountryListEnabled);
                FilterCommand.RaiseCanExecuteChanged();
            }
        }

        private readonly ObservableCollection<RegionDto> _regions;
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
                RaisePropertyChanged(() => Regions);

                // Select default region.
                SelectedRegion = _regions.FirstOrDefault(c => c.RegionId == -1);

                // Enable/disable control in UI.
                RaisePropertyChanged(() => IsRegionListEnabled);
                FilterCommand.RaiseCanExecuteChanged();
            }
        }

        private readonly ObservableCollection<CityDto> _cities;
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

                RaisePropertyChanged(() => Cities);

                // Select default city.
                SelectedCity = _cities.FirstOrDefault(c => c.CityId == -1);

                // Enable/disable control in UI.
                RaisePropertyChanged(() => IsCityListEnabled);
                FilterCommand.RaiseCanExecuteChanged();
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

                RaisePropertyChanged(() => SelectedCountry);
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

                RaisePropertyChanged(() => SelectedRegion);
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

                RaisePropertyChanged(() => SelectedRegion);

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

        public BreweryListFilterViewModel()
        {
            _continents = new ObservableCollection<ContinentDto>();
            _countries = new ObservableCollection<CountryDto>();
            _regions = new ObservableCollection<RegionDto>();
            _cities = new ObservableCollection<CityDto>();

            // Commands
            FilterCommand = new DelegateCommand(OnBreweryFilter, CanFilterBrewery);

            // Poopup message boxes.
            ShowMessagebox = new InteractionRequest<Notification>();

            if (!DesignerProperties.IsInDesignTool)
            {
                CompositionInitializer.SatisfyImports(this);

                // Subscribe to events.
                EventAggregator.GetEvent<BreweriesReloadEvent>().Subscribe(OnBreweryReloading);
            }

            GetContinents();
        }

        #endregion


        #region Commands methods

        public void OnBreweryFilter()
        {
            var breweryFilter = new BreweryFilter
            {
                ContinentId = _selectedContinent.ContinentId,
                CountryId = _selectedCountry.CountryId,
                RegionId = _selectedRegion.RegionId,
                CityId = _selectedCity.CityId
            };

            RegionManager.Regions["BreweryFilterContent"].Context = breweryFilter;

            EventAggregator.GetEvent<BreweryFilterEvent>().Publish(breweryFilter);
            EventAggregator.GetEvent<ShowBreweriesListRegionEvent>().Publish(true);
        }
        
        private bool CanFilterBrewery()
        {
            if (_selectedContinent == null || _selectedCountry == null || _selectedRegion == null || _selectedCity == null)
            {
                return false;
            }
            return true;
        }

        public void OnBreweryReloading(bool obj)
        {
            GetContinents();
        }

        #endregion


        #region Methods to communicate with database

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
    }
}
