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
using System.Windows.Media.Imaging;
using CapsCollection.Silverlight.Infrastructure.Events;
using CapsCollection.Silverlight.ServiceAgents.ProxiesWrappers;
using CapsCollection.Silverlight.UI.Modules.Services.Interfaces;

namespace CapsCollection.Silverlight.UI.Modules.Geography.ViewModels
{
    public class CountryListViewModel : ViewModelBase
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

        public DelegateCommand AddCountryCommand { get; private set; }
        public DelegateCommand EditCountryCommand { get; private set; }
        public DelegateCommand DeleteCountryCommand { get; private set; }

        #endregion


        #region Properties and Members

        public InteractionRequest<Notification> ShowMessagebox { get; set; }
        public InteractionRequest<Confirmation> ShowConfirmation { get; set; }

        ContinentDto _selectedContinent;
        public ContinentDto SelectedContinent
        {
            get { return _selectedContinent; }
            set
            {
                _selectedContinent = value;
                AddCountryCommand.RaiseCanExecuteChanged();
            }
        }

        private ObservableCollection<CountryWithFlags> _countries;
        public ObservableCollection<CountryWithFlags> Countries
        {
            get { return _countries; }
            set
            {
                _countries.Clear();
                _countries = value;
                RaisePropertyChanged(() => Countries);
            }
        }

        CountryWithFlags _selectedCountry;
        public CountryWithFlags SelectedCountry
        {
            get { return _selectedCountry; }
            set
            {
                if (_selectedCountry != value)
                {
                    _selectedCountry = value;
                    RaisePropertyChanged(() => SelectedCountry);
                    EditCountryCommand.RaiseCanExecuteChanged();
                    DeleteCountryCommand.RaiseCanExecuteChanged();
                    EventAggregator.GetEvent<CountrySelectedEvent>().Publish(_selectedCountry);
                }
            }
        }

        #endregion


        #region Constructors

        public CountryListViewModel()
        {
            // Commands.
            AddCountryCommand = new DelegateCommand(OnAddCountry, CanAddCountry);
            EditCountryCommand = new DelegateCommand(OnEditCountry, CanEditCountry);
            DeleteCountryCommand = new DelegateCommand(OnDeleteCountry, CanDeleteCountry);

            // Poopup message boxes.
            ShowMessagebox = new InteractionRequest<Notification>();
            ShowConfirmation = new InteractionRequest<Confirmation>();

            // Main list.
            _countries = new ObservableCollection<CountryWithFlags>();

            if (!DesignerProperties.IsInDesignTool)
            {
                CompositionInitializer.SatisfyImports(this);

                // When continent is selected call event to get countries from database.
                EventAggregator.GetEvent<ContinentSelectedEvent>().Subscribe(OnContinentSelected);

                // When country is added or updated change element in UI. 
                EventAggregator.GetEvent<CountryAddedEvent>().Subscribe(OnCountryAddedOrUpdated);
            }
        }

        #endregion


        #region Methods to communicate with database

        public void GetCountries(int continentId)
        {
            IsBusy = true;

            var client = new GeographyServiceClientWrapper();

            client.GetCountriesByContinentAsync(continentId, true);
            client.GetCountriesByContinentCompleted += delegate (object sender, GetCountriesByContinentCompletedEventArgs e)
            {
                Countries.Clear();

                if (e.Error == null && e.Result != null)
                {
                    foreach (var countryDto in e.Result)
                    {
                        var country = new CountryWithFlags
                        {
                            CountryId = countryDto.CountryId,
                            ContinentId = countryDto.ContinentId,
                            EnglishCountryName = countryDto.EnglishCountryName,
                            EnglishCountryFullName = countryDto.EnglishCountryFullName,
                            NationalCountryName = countryDto.NationalCountryName,
                            NationalCountryFullName = countryDto.NationalCountryFullName,
                            Alpha2 = countryDto.Alpha2,
                            Alpha3 = countryDto.Alpha3,
                            ISO = countryDto.ISO,
                            PreciseLocation = countryDto.PreciseLocation
                        };

                        // Create flag image.
                        country.CreateFlagFullImage(BitmapCreateOptions.DelayCreation, FlagType.Full);

                        _countries.Add(country);
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
        
        public void DeleteCountry(CountryWithFlags selectedCountry)
        {
            IsBusy = true;

            var client = new GeographyServiceClientWrapper();

            var countryDto = new CountryDto
            {
                CountryId = _selectedCountry.CountryId
            };

            client.DeleteCountryAsync(countryDto);
            client.DeleteCountryCompleted += delegate (object sender, AsyncCompletedEventArgs e)
            {
                if (e.Error == null)
                {
                    OnCountryDeleted(selectedCountry);
                }
                else if (e.Error is FaultException<ExceptionDetail>)
                {
                    var serviceFault = e.Error as FaultException<ExceptionDetail>;
                    
                    ShowMessagebox.Raise(new Notification
                    {
                        Title = "FaultException. Error deleting country.",
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
                        Title = "Exception. Error deleting country.",
                        Content = e.Error.Message
                    }, notification => { });
                }

                IsBusy = false;
            };
        }

        #endregion


        #region Buttons click command methods

        public void OnContinentSelected(ContinentDto continent)
        {
            SelectedContinent = continent;
            GetCountries(continent.ContinentId);
        }

        private void OnAddCountry()
        {
            var uriQuery = new UriQuery();
            if (SelectedContinent != null)
            {
                uriQuery.Add("continentId", SelectedContinent.ContinentId.ToString(CultureInfo.InvariantCulture));
            }

            var uri = new Uri("CountryEditView" + uriQuery, UriKind.Relative);

            RegionManager.RequestNavigate("PopupRegionContent", uri);
        }

        private bool CanAddCountry()
        {
            if (SelectedContinent == null)
                return false;

            return true;
        }

        private void OnEditCountry()
        {
            var uriQuery = new UriQuery();

            if (SelectedCountry != null)
            {
                uriQuery.Add("countryId", SelectedCountry.CountryId.ToString(CultureInfo.InvariantCulture));
            }

            if (SelectedContinent != null)
            {
                uriQuery.Add("continentId", SelectedContinent.ContinentId.ToString(CultureInfo.InvariantCulture));
            }

            var uri = new Uri("CountryEditView" + uriQuery, UriKind.Relative);

            RegionManager.RequestNavigate("PopupRegionContent", uri);
        }

        private bool CanEditCountry()
        {
            if (SelectedCountry == null)
                return false;

            return true;
        }

        private void OnDeleteCountry()
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

            ShowConfirmation.Raise(new Confirmation { Content = "Selected country will be deleted. Do you accept?", Title = "Delete confirmation" }, confirmation =>
            {
                if (confirmation.Confirmed)
                {
                    DeleteCountry(SelectedCountry);
                }
            });
        }

        private bool CanDeleteCountry()
        {
            if (SelectedCountry == null)
                return false;

            return true;
        }

        #endregion


        #region Methods to manipulate elements in list in UI

        public void OnCountryAddedOrUpdated(CountryWithFlags addedCountry)
        {
            if (addedCountry == null)
                return;

            // Add new country to the list or update selelcted country title and flag in UI.
            if (addedCountry.CountryId == 0)
            {
                GetCountries(addedCountry.ContinentId);
            }
            else
            {
                SelectedCountry.EnglishCountryName = addedCountry.EnglishCountryName;
                SelectedCountry.FlagFullImage = addedCountry.FlagFullImage;
            }
        }

        private void OnCountryDeleted(CountryWithFlags deletedCountry)
        {
            if (deletedCountry == null)
                return;

            // Remove country from the list in UI.
            Countries.Remove(deletedCountry);
        }

        #endregion
    }
}
