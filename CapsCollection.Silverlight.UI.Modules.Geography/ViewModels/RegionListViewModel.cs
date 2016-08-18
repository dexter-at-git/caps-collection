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
    public class RegionListViewModel : ViewModelBase
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

        public DelegateCommand AddRegionCommand { get; private set; }
        public DelegateCommand EditRegionCommand { get; private set; }
        public DelegateCommand DeleteRegionCommand { get; private set; }

        #endregion


        #region Properties and Members

        public InteractionRequest<Notification> ShowMessagebox { get; set; }
        public InteractionRequest<Confirmation> ShowConfirmation { get; set; }


        private CountryWithFlags _selectedCountry;
        public CountryWithFlags SelectedCountry
        {
            get { return _selectedCountry; }
            set
            {
                _selectedCountry = value;
                AddRegionCommand.RaiseCanExecuteChanged();
            }
        }

        private ObservableCollection<RegionDto> _regions;
        public ObservableCollection<RegionDto> Regions
        {
            get { return _regions; }
            set
            {
                _regions = value;
                RaisePropertyChanged(() => Regions);
            }
        }

        private RegionDto _selectedRegion;
        public RegionDto SelectedRegion
        {
            get { return _selectedRegion; }
            set
            {
                if (_selectedRegion != value)
                {
                    _selectedRegion = value;
                    RaisePropertyChanged(() => SelectedRegion);
                    EditRegionCommand.RaiseCanExecuteChanged();
                    DeleteRegionCommand.RaiseCanExecuteChanged();
                    EventAggregator.GetEvent<RegionSelectedEvent>().Publish(_selectedRegion);
                }
            }
        }

        private bool _isHidden;
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

        public RegionListViewModel()
        {
            // At first load region list is hidden.
            IsHidden = true;

            // Commands.
            AddRegionCommand = new DelegateCommand(OnAddRegion, CanAddRegion);
            EditRegionCommand = new DelegateCommand(OnEditRegion, CanEditRegion);
            DeleteRegionCommand = new DelegateCommand(OnDeleteRegion, CanDeleteRegion);

            // Poopup message boxes.
            ShowMessagebox = new InteractionRequest<Notification>();
            ShowConfirmation = new InteractionRequest<Confirmation>();

            // Main list.
            _regions = new ObservableCollection<RegionDto>();

            if (!DesignerProperties.IsInDesignTool)
            {
                CompositionInitializer.SatisfyImports(this);

                // When country is selected call event to get regions from database.
                EventAggregator.GetEvent<CountrySelectedEvent>().Subscribe(OnCountrySelected);

                // When region is added or updated change element in UI. 
                EventAggregator.GetEvent<RegionAddedEvent>().Subscribe(OnRegionAddedOrUpdated);

                // When continent is selected hide region list.
                EventAggregator.GetEvent<ContinentSelectedEvent>().Subscribe(OnContinentSelected);
            }
        }

        #endregion


        #region Methods to communicate with database

        private void GetCountryRegions(CountryWithFlags country)
        {
            IsBusy = true;

            var client = new GeographyServiceClientWrapper();

            client.GetRegionsByCountryAsync(country.CountryId);
            client.GetRegionsByCountryCompleted += delegate (object sender, GetRegionsByCountryCompletedEventArgs e)
            {
                if (e.Error == null)
                {
                    var listRegions = e.Result;

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

        public void DeleteRegion(RegionDto selectedRegion)
        {
            IsBusy = true;

            var client = new GeographyServiceClientWrapper();

            client.DeleteRegionAsync(selectedRegion);
            client.DeleteRegionCompleted += delegate (object sender, AsyncCompletedEventArgs e)
            {
                if (e.Error == null)
                {
                    OnRegionDeleted(selectedRegion);
                }
                else if (e.Error is FaultException<ExceptionDetail>)
                {
                    var serviceFault = e.Error as FaultException<ExceptionDetail>;

                    var innerException = serviceFault.Detail.InnerException.InnerException.Message;
                    
                    ShowMessagebox.Raise(new Notification
                    {
                        Title = "FaultException. Error deleting region.",
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
                        Title = "Exception. Error deleting region.",
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
            IsHidden = true;
            if (_regions != null)
                _regions.Clear();
        }

        public void OnCountrySelected(CountryWithFlags country)
        {
            SelectedCountry = country;
            if (country != null)
            {
                GetCountryRegions(country);
                IsHidden = false;
            }
        }

        private void OnAddRegion()
        {
            var uriQuery = new UriQuery();
            if (SelectedCountry != null)
            {
                uriQuery.Add("countryId", SelectedCountry.CountryId.ToString(CultureInfo.InvariantCulture));
            }

            var uri = new Uri("RegionEditView" + uriQuery, UriKind.Relative);

            RegionManager.RequestNavigate("PopupRegionContent", uri);
        }

        private bool CanAddRegion()
        {
            if (SelectedCountry == null)
                return false;

            return true;
        }

        private void OnEditRegion()
        {
            var uriQuery = new UriQuery();

            if (SelectedRegion != null)
            {
                uriQuery.Add("regionId", SelectedRegion.RegionId.ToString(CultureInfo.InvariantCulture));
            }

            var uri = new Uri("RegionEditView" + uriQuery, UriKind.Relative);

            RegionManager.RequestNavigate("PopupRegionContent", uri);
        }

        private bool CanEditRegion()
        {
            if (SelectedRegion == null)
                return false;

            return true;
        }

        private void OnDeleteRegion()
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

            ShowConfirmation.Raise(new Confirmation { Content = "Selected region will be deleted. Do you accept?", Title = "Delete confirmation" }, confirmation =>
            {
                if (confirmation.Confirmed)
                {
                    DeleteRegion(SelectedRegion);
                }
            });
        }

        private bool CanDeleteRegion()
        {
            if (SelectedRegion == null)
                return false;

            return true;
        }

        #endregion


        #region Methods to manipulate elements in list in UI

        public void OnRegionAddedOrUpdated(RegionDto addedRegion)
        {
            if (addedRegion == null) return;

            // Add new region to the list or update selelcted region title in UI.
            if (addedRegion.RegionId == 0)
            {
                GetCountryRegions(SelectedCountry);
            }
            else
            {
                SelectedRegion.EnglishRegionName = addedRegion.EnglishRegionName;
            }
        }

        private void OnRegionDeleted(RegionDto deletedRegion)
        {
            if (deletedRegion == null) return;

            // Remove region from the list in UI.
            Regions.Remove(deletedRegion);
        }

        #endregion
    }
}
