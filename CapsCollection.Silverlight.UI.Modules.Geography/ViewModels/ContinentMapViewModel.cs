using CapsCollection.Silverlight.Infrastructure.ViewModels;
using CapsCollection.Silverlight.ServiceAgents.Proxies.Geography;
using CapsCollection.Silverlight.UI.Modules.Geography.Views;
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
using System.Windows;
using System.Windows.Media;
using CapsCollection.Silverlight.Infrastructure.Events;
using CapsCollection.Silverlight.ServiceAgents.ProxiesWrappers;

namespace CapsCollection.Silverlight.UI.Modules.Geography.ViewModels
{
    public class ContinentMapViewModel : ViewModelBase
    {
        #region MEF imports

        [Import]
        public IRegionManager RegionManager { get; set; }
        [Import]
        public IEventAggregator EventAggregator { get; set; }

        #endregion


        #region Commands

        public DelegateCommand<string> OnMouseEnterCommand { get; private set; }
        public DelegateCommand<string> OnMouseLeaveCommand { get; private set; }
        public DelegateCommand<string> OnMouseLeftButtonUpCommand { get; private set; }

        #endregion


        #region Properties and Members

        public InteractionRequest<Notification> ShowMessagebox { get; set; }

        private ObservableCollection<ContinentDto> _continents;
        public ObservableCollection<ContinentDto> Continents
        {
            get { return _continents; }
            set
            {
                _continents.Clear();
                _continents = value;
                RaisePropertyChanged("Continents");
            }
        }

        private ContinentDto _selectedContinent;
        public ContinentDto SelectedContinent
        {
            get { return _selectedContinent; }
            set
            {
                if (_selectedContinent != value)
                {
                    _selectedContinent = value;
                    RaisePropertyChanged(() => SelectedContinent);
                    EventAggregator.GetEvent<ContinentSelectedEvent>().Publish(_selectedContinent);
                }
            }
        }

        private SolidColorBrush _selectedCountryBrush;
        public SolidColorBrush SelectedCountryBrush
        {
            get { return _selectedCountryBrush; }
            set
            {
                if (_selectedCountryBrush != value)
                {
                    _selectedCountryBrush = value;
                    RaisePropertyChanged("SelectedCountryBrush");
                }
            }
        }

        #endregion


        #region Constructors

        public ContinentMapViewModel()
        {
            IsBusy = true;

            _selectedCountryBrush = new SolidColorBrush(Color.FromArgb(255, 220, 220, 220));

            // Commands
            OnMouseEnterCommand = new DelegateCommand<string>(OnMouseEnter);
            OnMouseLeaveCommand = new DelegateCommand<string>(OnMouseLeave);
            OnMouseLeftButtonUpCommand = new DelegateCommand<string>(OnMouseLeftButtonUp);

            // Poopup message boxes.
            ShowMessagebox = new InteractionRequest<Notification>();

            // Main list.
            _continents = new ObservableCollection<ContinentDto>();

            if (!DesignerProperties.IsInDesignTool)
            {
                GetContinents();
                CompositionInitializer.SatisfyImports(this);
            }
        }

        private void OnMouseLeftButtonUp(string parameter)
        {
            SelectedContinent = Continents.FirstOrDefault(x => x.ContinentCode == parameter);

            var uriQuery = new UriQuery();
            if (SelectedContinent != null)
            {
                uriQuery.Add("continentId", SelectedContinent.ContinentId.ToString(CultureInfo.InvariantCulture));
            }
            var uri = new Uri("CountryListView" + uriQuery, UriKind.Relative);

            RegionManager.RequestNavigate("CountryContent", uri);

            IRegion popupRegion = RegionManager.Regions["PopupRegionContent"];
            foreach (var view in popupRegion.Views)
            {
                if (view is ContinentMapView && ((FrameworkElement)view).DataContext == this)
                {
                    popupRegion.Remove(view);
                    break;
                }
            }
        }

        private void OnMouseLeave(string parameter)
        {
            SelectedCountryBrush = new SolidColorBrush(Color.FromArgb(255, 220, 220, 220));
        }

        private void OnMouseEnter(string parameter)
        {
            switch (parameter)
            {
                case "AN":
                    SelectedCountryBrush = new SolidColorBrush(Color.FromArgb(255, 0, 64, 255));
                    break;
                case "NA":
                    SelectedCountryBrush = new SolidColorBrush(Color.FromArgb(255, 0, 204, 0));
                    break;
                case "SA":
                    SelectedCountryBrush = new SolidColorBrush(Color.FromArgb(255, 0, 128, 0));
                    break;
                case "AS":
                    SelectedCountryBrush = new SolidColorBrush(Color.FromArgb(255, 243, 62, 1));
                    break;
                case "AF":
                    SelectedCountryBrush = new SolidColorBrush(Color.FromArgb(255, 254, 213, 46));
                    break;
                case "EU":
                    SelectedCountryBrush = new SolidColorBrush(Color.FromArgb(255, 193, 0, 0));
                    break;
                case "OC":
                    SelectedCountryBrush = new SolidColorBrush(Color.FromArgb(255, 192, 64, 128));
                    break;
            }
        }

        #endregion


        #region Methods to communicate with database

        public void GetContinents()
        {
            var client = new GeographyServiceClientWrapper();

            client.GetContinentsCompleted += delegate (object sender, GetContinentsCompletedEventArgs e)
            {
                if (e.Error == null)
                {
                    var continents = e.Result;

                    _continents = continents;
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

            client.GetContinentsAsync();
        }

        #endregion
    }
}
