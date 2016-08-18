using System.Windows.Media.Imaging;
using CapsCollection.Silverlight.Infrastructure.Models;
using CapsCollection.Silverlight.Infrastructure.ViewModels;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Regions;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.ServiceModel;
using CapsCollection.Silverlight.Infrastructure.Events;
using CapsCollection.Silverlight.ServiceAgents.Proxies.Geography;
using CapsCollection.Silverlight.ServiceAgents.ProxiesWrappers;

namespace CapsCollection.Silverlight.UI.Modules.Collection.ViewModels
{
    public class BeerCountriesListViewModel : ViewModelBase
    {
        #region MEF imports

        [Import]
        public IRegionManager RegionManager { get; set; }
        [Import]
        public IEventAggregator EventAggregator { get; set; }

        #endregion


        #region Properties and Members

        public InteractionRequest<Notification> ShowMessagebox { get; set; }
        public InteractionRequest<Confirmation> ShowConfirmation { get; set; }

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
                    EventAggregator.GetEvent<BeerCountrySelectedEvent>().Publish(_selectedCountry);
                    EventAggregator.GetEvent<ShowBeerListRegionEvent>().Publish(true);
                }
            }
        }

        #endregion


        #region Constructors

        public BeerCountriesListViewModel()
        {
            // Poopup message boxes.
            ShowMessagebox = new InteractionRequest<Notification>();
            ShowConfirmation = new InteractionRequest<Confirmation>();

            // Main list with countries.
            _countries = new ObservableCollection<CountryWithFlags>();

            if (!DesignerProperties.IsInDesignTool)
            {
                CompositionInitializer.SatisfyImports(this);

                // Subscribe to events.
                EventAggregator.GetEvent<CountryBeerAddedEvent>().Subscribe(OnBeerAdded);
                EventAggregator.GetEvent<CountryBeerDeletedEvent>().Subscribe(OnBeerDeleted);
                EventAggregator.GetEvent<BeerCountriesReloadEvent>().Subscribe(OnBeerReloading);
            }

            // Get countries from database.
            GetBeerCountries();
        }

        #endregion


        #region Methods to communicate with database

        public void GetBeerCountries()
        {
            IsBusy = true;

            var client = new GeographyServiceClientWrapper();

            client.GetBeerCountriesAsync();
            client.GetBeerCountriesCompleted += delegate (object sender, GetBeerCountriesCompletedEventArgs e)
            {
                Countries.Clear();

                if (e.Error == null && e.Result != null)
                {
                    foreach (var beerCountryDto in e.Result)
                    {
                        var country = new CountryWithFlags
                        {
                            CountryId = beerCountryDto.CountryId,
                            EnglishCountryName = beerCountryDto.EnglishCountryName,
                            BeerCount = beerCountryDto.BeerCount,
                            Alpha3 = beerCountryDto.Alpha3
                        };
                        country.CreateFlagRoundImage(BitmapCreateOptions.DelayCreation, FlagType.Round);
                        _countries.Add(country);
                    }
                }
                else if (e.Error is FaultException<ExceptionDetail>)
                {
                    var serviceFault = e.Error as FaultException<ExceptionDetail>;
                    ShowMessagebox.Raise(new Notification
                    {
                        Title = "FaultException. Error getting countries.",
                        Content = serviceFault.Detail.Message + Environment.NewLine + serviceFault.Detail.InnerException
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

        #endregion


        #region Event methods

        public void OnBeerAdded(int addedBeerCountryId)
        {
            // Find country in the list.
            var country = Countries.FirstOrDefault(c => c.CountryId == addedBeerCountryId);

            if (country == null)
            {
                // New country, so reload list. And get country from database.
                EventAggregator.GetEvent<ShowBeerListRegionEvent>().Publish(false);
                EventAggregator.GetEvent<ShowBeerEditRegionEvent>().Publish(false);
                GetBeerCountries();
            }
            else
            {
                // Increment beer count in country.
                country.BeerCount++;
            }
        }

        public void OnBeerDeleted(int deletedBeerCountryId)
        {
            // Find country in the list.
            var country = Countries.FirstOrDefault(c => c.CountryId == deletedBeerCountryId);

            if (country == null) return;

            // Decrement beer count in country. 
            country.BeerCount--;

            if (country.BeerCount != 0)
                return;

            // No more beers in cuntry. Remove it from the list.
            Countries.Remove(country);

            // Hide regions.
            EventAggregator.GetEvent<ShowBeerListRegionEvent>().Publish(false);
            EventAggregator.GetEvent<ShowBeerEditRegionEvent>().Publish(false);
        }


        public void OnBeerReloading(bool obj)
        {
            GetBeerCountries();
        }

        #endregion
    }
}
