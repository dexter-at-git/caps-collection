using CapsCollection.Silverlight.Infrastructure.ViewModels;
using CapsCollection.Silverlight.ServiceAgents.Proxies.Beer;
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
using System.Linq;
using System.ServiceModel;
using CapsCollection.Silverlight.Infrastructure.Events;
using CapsCollection.Silverlight.Infrastructure.Models;
using CapsCollection.Silverlight.ServiceAgents.ProxiesWrappers;

namespace CapsCollection.Silverlight.UI.Modules.Brewery.ViewModels
{
    public class BreweryListViewModel : ViewModelBase
    {
        #region MEF imports

        [Import]
        public IRegionManager RegionManager { get; set; }
        [Import]
        public IEventAggregator EventAggregator { get; set; }

        #endregion


        #region Commands and Events

        public DelegateCommand EditBreweryCommand { get; private set; }
        public DelegateCommand DeleteBreweryCommand { get; private set; }

        #endregion


        #region Properties and Members

        public InteractionRequest<Notification> ShowMessagebox { get; set; }
        public InteractionRequest<Confirmation> ShowConfirmation { get; set; }

        BreweryDto _selectedBrewery;
        public BreweryDto SelectedBrewery
        {
            get { return _selectedBrewery; }
            set
            {
                _selectedBrewery = value;
                EditBreweryCommand.RaiseCanExecuteChanged();
                DeleteBreweryCommand.RaiseCanExecuteChanged();
            }
        }

        BreweryFilter _breweryFilter;
        public BreweryFilter BreweryFilter
        {
            get { return _breweryFilter; }
            set
            {
                _breweryFilter = value;
            }
        }

        private ObservableCollection<BreweryDto> _breweries;
        public ObservableCollection<BreweryDto> Breweries
        {
            get { return _breweries; }
            set
            {
                _breweries.Clear();
                _breweries = value;
                RaisePropertyChanged(() => Breweries);
            }
        }

        #endregion


        #region Constructors

        public BreweryListViewModel()
        {
            // Commands.
            EditBreweryCommand = new DelegateCommand(OnEditBrewery, CanEditBrewery);
            DeleteBreweryCommand = new DelegateCommand(OnDeleteBrewery, CanDeleteBrewery);

            // Poopup message boxes.
            ShowMessagebox = new InteractionRequest<Notification>();
            ShowConfirmation = new InteractionRequest<Confirmation>();

            // Main list with breweries.
            _breweries = new ObservableCollection<BreweryDto>();

            if (!DesignerProperties.IsInDesignTool)
            {
                CompositionInitializer.SatisfyImports(this);

                // When country is selected call event to get regions from database.
                EventAggregator.GetEvent<BreweryFilterEvent>().Subscribe(OnBreweryFilter);

                // When brewery is added or updated change element in UI. 
                EventAggregator.GetEvent<BreweryAddedEvent>().Subscribe(OnBreweryAddedOrUpdated);
            }
        }

        #endregion


        #region Command events

        public void OnBreweryFilter(BreweryFilter breweryFilter)
        {
            if (breweryFilter != null)
            {
                BreweryFilter = breweryFilter;
                GetBreweries(breweryFilter.ContinentId, breweryFilter.CountryId, breweryFilter.RegionId, breweryFilter.CityId);
            }
        }

        private void OnEditBrewery()
        {
            var uriQuery = new UriQuery();

            if (SelectedBrewery != null)
            {
                uriQuery.Add("breweryId", SelectedBrewery.BreweryId.ToString(CultureInfo.InvariantCulture));
            }

            var uri = new Uri("BreweryEditView" + uriQuery, UriKind.Relative);

            RegionManager.RequestNavigate("EditBreweryContent", uri);

            EventAggregator.GetEvent<ShowBreweriesEditRegionEvent>().Publish(true);
        }

        private bool CanEditBrewery()
        {
            if (SelectedBrewery == null)
                return false;

            return true;
        }

        private void OnDeleteBrewery()
        {
            ShowConfirmation.Raise(new Confirmation { Content = "Selected brewery will be deleted. Do you accept?", Title = "Delete confirmation" }, confirmation =>
            {
                if (confirmation.Confirmed)
                {
                    DeleteBrewery(SelectedBrewery);
                }
            });
        }

        private bool CanDeleteBrewery()
        {
            if (SelectedBrewery == null)
                return false;

            return true;
        }

        #endregion


        #region Methods to communicate with database

        public void GetBreweries(int continentId, int countryId, int regionId, int cityId)
        {
            IsBusy = true;

            var client = new BeerServiceClientWrapper();

            client.GetBreweriesByFilterAsync(continentId, countryId, regionId, cityId);
            client.GetBreweriesByFilterCompleted += delegate (object sender, GetBreweriesByFilterCompletedEventArgs e)
            {
                Breweries.Clear();

                if (e.Error == null && e.Result != null)
                {
                    ObservableCollection<BreweryDto> listBreweries = e.Result;

                    Breweries = listBreweries;
                }
                else if (e.Error is FaultException<ExceptionDetail>)
                {
                    var serviceFault = e.Error as FaultException<ExceptionDetail>;
                    
                    ShowMessagebox.Raise(new Notification
                    {
                        Title = "FaultException. Error getting breweries.",
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
                        Title = "Exception. Error getting breweries.",
                        Content = e.Error.Message
                    }, notification => { });
                }

                IsBusy = false;
            };
        }


        public void DeleteBrewery(BreweryDto selectedBrewery)
        {
            IsBusy = true;

            var client = new BeerServiceClientWrapper();

            client.DeleteBreweryAsync(selectedBrewery);
            client.DeleteBreweryCompleted += delegate (object sender, AsyncCompletedEventArgs e)
            {
                if (e.Error == null)
                {
                    OnBreweryDeleted(selectedBrewery);
                }
                else if (e.Error is FaultException<ExceptionDetail>)
                {
                    var serviceFault = e.Error as FaultException<ExceptionDetail>;

                    var innerException = serviceFault.Detail.InnerException.InnerException.Message;
                    
                    ShowMessagebox.Raise(new Notification
                    {
                        Title = "FaultException. Error deleting brewery.",
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
                        Title = "Exception. Error deleting brewery.",
                        Content = e.Error.Message
                    }, notification => { });
                }

                IsBusy = false;
            };
        }

        #endregion


        #region Methods to manipulate elements in list in UI

        public void OnBreweryAddedOrUpdated(BreweryDto addedBrewery)
        {
            if (addedBrewery == null) return;

            // Refresh brewery list or update selelcted brewery title and flag in UI.
            if (addedBrewery.BreweryId == 0)
            {
                GetBreweries(BreweryFilter.ContinentId, BreweryFilter.CountryId, BreweryFilter.RegionId, BreweryFilter.CityId);
            }
            else
            {
                var brewery = Breweries.FirstOrDefault(x => x.BreweryId == addedBrewery.BreweryId);
                if (brewery != null)
                    brewery.Brewery = addedBrewery.Brewery;
            }
        }

        private void OnBreweryDeleted(BreweryDto deletedBrewery)
        {
            if (deletedBrewery == null) return;

            // Remove brewery from the list in UI.
            Breweries.Remove(deletedBrewery);
        }

        #endregion
    }
}
