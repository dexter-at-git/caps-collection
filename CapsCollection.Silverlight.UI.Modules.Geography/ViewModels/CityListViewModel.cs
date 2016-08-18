using CapsCollection.Silverlight.Infrastructure.Models;
using CapsCollection.Silverlight.Infrastructure.ViewModels;
using CapsCollection.Silverlight.ServiceAgents.Proxies.Geography;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Regions;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Globalization;
using System.ServiceModel;
using CapsCollection.Silverlight.Infrastructure.Events;
using CapsCollection.Silverlight.ServiceAgents.ProxiesWrappers;
using CapsCollection.Silverlight.UI.Modules.Services.Interfaces;

namespace CapsCollection.Silverlight.UI.Modules.Geography.ViewModels
{
    public class CityListViewModel : ViewModelBase
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

        public DelegateCommand AddCityCommand { get; private set; }
        public DelegateCommand EditCityCommand { get; private set; }
        public DelegateCommand DeleteCityCommand { get; private set; }

        #endregion


        #region Properties and Members

        public InteractionRequest<Notification> ShowMessagebox { get; set; }
        public InteractionRequest<Confirmation> ShowConfirmation { get; set; }


        private RegionDto _selectedRegion;
        public RegionDto SelectedRegion
        {
            get { return _selectedRegion; }
            set
            {
                _selectedRegion = value;
                AddCityCommand.RaiseCanExecuteChanged();
            }
        }

        private ObservableCollection<CityDto> _cities;
        public ObservableCollection<CityDto> Cities
        {
            get { return _cities; }
            set
            {
                _cities = value;
                RaisePropertyChanged(() => Cities);
            }
        }

        private CityDto _selectedCity;
        public CityDto SelectedCity
        {
            get { return _selectedCity; }
            set
            {
                if (_selectedCity != value)
                {
                    _selectedCity = value;
                    RaisePropertyChanged(() => SelectedCity);
                    EditCityCommand.RaiseCanExecuteChanged();
                    DeleteCityCommand.RaiseCanExecuteChanged();
                }
            }
        }

        bool _isHidden;
        public bool IsHidden
        {
            get { return _isHidden; }
            set
            {
                _isHidden = value;
                RaisePropertyChanged(() => IsHidden);
            }
        }

        #endregion


        #region Constructors

        public CityListViewModel()
        {
            // At first load city list is hidden.
            IsHidden = true;

            // Commands.
            AddCityCommand = new DelegateCommand(OnAddCity, CanAddCity);
            EditCityCommand = new DelegateCommand(OnEditCity, CanEditCity);
            DeleteCityCommand = new DelegateCommand(OnDeleteCity, CanDeleteCity);

            // Poopup message boxes.
            ShowMessagebox = new InteractionRequest<Notification>();
            ShowConfirmation = new InteractionRequest<Confirmation>();

            // Main list.
            _cities = new ObservableCollection<CityDto>();

            if (!DesignerProperties.IsInDesignTool)
            {
                CompositionInitializer.SatisfyImports(this);

                // When region is selected call event to get cities from database.
                EventAggregator.GetEvent<RegionSelectedEvent>().Subscribe(OnRegionSelected);

                // When region is added or updated change element in UI. 
                EventAggregator.GetEvent<CityAddedEvent>().Subscribe(OnCityAddedOrUpdated);

                // When country is selected hide city list.
                EventAggregator.GetEvent<CountrySelectedEvent>().Subscribe(OnCountrySelected);

                // When continent is selected hide city list.
                EventAggregator.GetEvent<ContinentSelectedEvent>().Subscribe(OnContinentSelected);
            }
        }

        #endregion


        #region Methods to communicate with database

        private void GetRegionCities(RegionDto region)
        {
            IsBusy = true;

            var client = new GeographyServiceClientWrapper();

            client.GetCitiesByRegionAsync(region.RegionId);
            client.GetCitiesByRegionCompleted += delegate (object sender, GetCitiesByRegionCompletedEventArgs e)
            {
                if (e.Error == null)
                {
                    var listCities = e.Result;

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

        public void DeleteCity(CityDto selectedCity)
        {
            IsBusy = true;

            var client = new GeographyServiceClientWrapper();

            client.DeleteCityAsync(selectedCity);
            client.DeleteCityCompleted += delegate (object sender, AsyncCompletedEventArgs e)
            {
                if (e.Error == null)
                {
                    OnCityDeleted(selectedCity);
                }
                else if (e.Error is FaultException<ExceptionDetail>)
                {
                    var serviceFault = e.Error as FaultException<ExceptionDetail>;

                    var innerException = serviceFault.Detail.InnerException.InnerException.Message;
                    
                    ShowMessagebox.Raise(new Notification
                    {
                        Title = "FaultException. Error deleting city.",
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
                        Title = "Exception. Error deleting city.",
                        Content = e.Error.Message
                    }, notification => { });
                }

                IsBusy = false;
            };
        }

        #endregion


        #region Buttons click command methods

        public void OnContinentSelected(ContinentDto obj)
        {
            IsHidden = true;
            _cities.Clear();
        }

        public void OnCountrySelected(CountryWithFlags obj)
        {
            IsHidden = true;
            if (_cities != null)
                _cities.Clear();
        }

        public void OnRegionSelected(RegionDto region)
        {
            SelectedRegion = region;
            if (region != null)
            {
                GetRegionCities(region);
                IsHidden = false;
            }
        }

        private void OnAddCity()
        {
            var uriQuery = new UriQuery();
            if (SelectedRegion != null)
            {
                uriQuery.Add("regionId", SelectedRegion.RegionId.ToString(CultureInfo.InvariantCulture));
            }

            var uri = new Uri("CityEditView" + uriQuery, UriKind.Relative);

            RegionManager.RequestNavigate("PopupRegionContent", uri);
        }

        private bool CanAddCity()
        {
            if (SelectedRegion == null)
                return false;

            return true;
        }

        private void OnEditCity()
        {
            var uriQuery = new UriQuery();

            if (SelectedCity != null)
            {
                uriQuery.Add("cityId", SelectedCity.CityId.ToString(CultureInfo.InvariantCulture));
            }

            var uri = new Uri("CityEditView" + uriQuery, UriKind.Relative);

            RegionManager.RequestNavigate("PopupRegionContent", uri);
        }

        private bool CanEditCity()
        {
            if (SelectedCity == null)
                return false;

            return true;
        }

        private void OnDeleteCity()
        {
            IsAuthenticated = AuthenticationManager.AuthenticationInfo.IsAuthenticated;
            if (!IsAuthenticated)
            {
                ShowMessagebox.Raise(new Notification
                {
                    Content = "Please, authenticate first.",
                    Title = "Delete is forbidden"
                });
                return;
            }

            ShowConfirmation.Raise(new Confirmation { Content = "Selected city will be deleted. Do you accept?", Title = "Delete confirmation" }, confirmation =>
            {
                if (confirmation.Confirmed)
                {
                    DeleteCity(SelectedCity);
                }
            });
        }

        private bool CanDeleteCity()
        {
            if (SelectedCity == null)
                return false;

            return true;
        }

        #endregion


        #region Methods to manipulate elements in list in UI

        public void OnCityAddedOrUpdated(CityDto addedCity)
        {
            if (addedCity == null)
                return;

            // Add new region to the list or update selelcted region title in UI.
            if (addedCity.CityId == 0)
            {
                GetRegionCities(SelectedRegion);
            }
            else
            {
                SelectedCity.EnglishCityName = addedCity.EnglishCityName;
            }
        }

        private void OnCityDeleted(CityDto deletedCity)
        {
            if (deletedCity == null)
                return;

            // Remove region from the list in UI.
            Cities.Remove(deletedCity);
        }

        #endregion
    }
}
