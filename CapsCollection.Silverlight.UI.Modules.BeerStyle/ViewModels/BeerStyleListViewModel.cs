using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Globalization;
using System.ServiceModel;
using CapsCollection.Silverlight.Infrastructure.Events;
using CapsCollection.Silverlight.Infrastructure.ViewModels;
using CapsCollection.Silverlight.ServiceAgents.Proxies.Beer;
using CapsCollection.Silverlight.ServiceAgents.ProxiesWrappers;
using CapsCollection.Silverlight.UI.Modules.Services.Interfaces;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Regions;

namespace CapsCollection.Silverlight.UI.Modules.BeerStyle.ViewModels
{
    public class BeerStyleListViewModel : ViewModelBase
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

        public DelegateCommand AddBeerStyleCommand { get; private set; }
        public DelegateCommand EditBeerStyleCommand { get; private set; }
        public DelegateCommand DeleteBeerStyleCommand { get; private set; }

        #endregion


        #region Properties and Members

        public InteractionRequest<Notification> ShowMessagebox { get; set; }
        public InteractionRequest<Confirmation> ShowConfirmation { get; set; }


        BeerStyleDto _selectedBeerStyle;
        public BeerStyleDto SelectedBeerStyle
        {
            get { return _selectedBeerStyle; }
            set
            {
                _selectedBeerStyle = value;
                EditBeerStyleCommand.RaiseCanExecuteChanged();
                DeleteBeerStyleCommand.RaiseCanExecuteChanged();
            }
        }


        private ObservableCollection<BeerStyleDto> _beerStyles;
        public ObservableCollection<BeerStyleDto> BeerStyles
        {
            get { return _beerStyles; }
            set
            {
                _beerStyles.Clear();
                _beerStyles = value;
                RaisePropertyChanged(() => BeerStyles);
            }
        }

        #endregion


        #region Constructors

        public BeerStyleListViewModel()
        {
            // Commands.
            EditBeerStyleCommand = new DelegateCommand(OnEditBeerStyle, CanEditBeerStyle);
            DeleteBeerStyleCommand = new DelegateCommand(OnDeleteBeerStyle, CanDeleteBeerStyle);

            // Poopup message boxes.
            ShowMessagebox = new InteractionRequest<Notification>();
            ShowConfirmation = new InteractionRequest<Confirmation>();

            // Main list.
            _beerStyles = new ObservableCollection<BeerStyleDto>();

            if (!DesignerProperties.IsInDesignTool)
            {
                CompositionInitializer.SatisfyImports(this);

                // When beer style is added or updated change element in UI. 
                EventAggregator.GetEvent<BeerStyleAddedEvent>().Subscribe(OnBeerStyleAddedOrUpdated);
            }

            GetBeerStyles();
        }

        #endregion


        #region Methods to communicate with database

        public void GetBeerStyles()
        {
            IsBusy = true;

            var client = new BeerServiceClientWrapper();

            client.GetBeerStylesAsync();
            client.GetBeerStylesCompleted += delegate (object sender, GetBeerStylesCompletedEventArgs e)
            {
                BeerStyles.Clear();

                if (e.Error == null)
                {
                    var listBeerStyles = e.Result;

                    BeerStyles = listBeerStyles;
                }
                else if (e.Error is FaultException<ExceptionDetail>)
                {
                    var serviceFault = e.Error as FaultException<ExceptionDetail>;
                    
                    ShowMessagebox.Raise(new Notification
                    {
                        Title = "FaultException. Error getting beer styles.",
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
                        Title = "Exception. Error getting beer styles.",
                        Content = e.Error.Message
                    }, notification => { });
                }

                IsBusy = false;
            };
        }
        

        public void DeleteBeerStyle(BeerStyleDto selectedBeerStyle)
        {
            IsBusy = true;

            var client = new BeerServiceClientWrapper();

            client.DeleteBeerStyleAsync(selectedBeerStyle);

            client.DeleteBeerStyleCompleted += delegate (object sender, AsyncCompletedEventArgs e)
            {
                if (e.Error == null)
                {
                    OnBeerStyleDeleted(selectedBeerStyle);
                }
                else if (e.Error is FaultException<ExceptionDetail>)
                {
                    var serviceFault = e.Error as FaultException<ExceptionDetail>;
                    
                    ShowMessagebox.Raise(new Notification
                    {
                        Title = "FaultException. Error deleting beer style.",
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
                        Title = "Exception. Error deleting beer style.",
                        Content = e.Error.Message
                    }, notification => { });
                }

                IsBusy = false;
            };
        }

        #endregion


        #region Buttons click command methods

        private void OnEditBeerStyle()
        {
            var uriQuery = new UriQuery();

            if (SelectedBeerStyle != null)
            {
                uriQuery.Add("beerStyleId", SelectedBeerStyle.BeerStyleId.ToString(CultureInfo.InvariantCulture));
            }

            var uri = new Uri("BeerStyleEditView" + uriQuery, UriKind.Relative);

            RegionManager.RequestNavigate("PopupRegionContent", uri);
        }

        private bool CanEditBeerStyle()
        {
            if (SelectedBeerStyle == null)
                return false;

            return true;
        }


        private void OnDeleteBeerStyle()
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

            ShowConfirmation.Raise(new Confirmation { Content = "Selected beer style will be deleted. Do you accept?", Title = "Delete confirmation" }, confirmation =>
            {
                if (confirmation.Confirmed)
                {
                    DeleteBeerStyle(SelectedBeerStyle);
                }
            });
        }

        private bool CanDeleteBeerStyle()
        {
            if (SelectedBeerStyle == null)
                return false;

            return true;
        }

        #endregion


        #region Methods to manipulate elements in list in UI

        public void OnBeerStyleAddedOrUpdated(BeerStyleDto addedBeerStyle)
        {
            if (addedBeerStyle == null)
                return;

            // Add new beer style to the list or update selelcted beer style title.
            if (addedBeerStyle.BeerStyleId == 0)
            {
                GetBeerStyles();
            }
            else
            {
                SelectedBeerStyle.BeerStyleName = addedBeerStyle.BeerStyleName;
            }
        }

        private void OnBeerStyleDeleted(BeerStyleDto deletedBeerStyle)
        {
            if (deletedBeerStyle == null)
                return;

            // Remove beer style from the list in UI.
            BeerStyles.Remove(deletedBeerStyle);
        }

        #endregion
    }
}
