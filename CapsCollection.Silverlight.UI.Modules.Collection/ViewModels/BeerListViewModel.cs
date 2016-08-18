using CapsCollection.Silverlight.Infrastructure.Models;
using CapsCollection.Silverlight.Infrastructure.ViewModels;
using CapsCollection.Silverlight.ServiceAgents.Proxies.Beer;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Regions;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.ServiceModel;
using System.Windows.Media.Imaging;
using CapsCollection.Silverlight.Infrastructure.Events;
using CapsCollection.Silverlight.ServiceAgents.ProxiesWrappers;

namespace CapsCollection.Silverlight.UI.Modules.Collection.ViewModels
{
    public class BeerListViewModel : ViewModelBase
    {
        #region MEF imports

        [Import]
        public IRegionManager RegionManager { get; set; }
        [Import]
        public IEventAggregator EventAggregator { get; set; }

        #endregion


        #region Commands and Events

        public DelegateCommand<BeerViewModel> SelectBeerCommand { get; private set; }

        #endregion


        #region Properties and Members

        public InteractionRequest<Notification> ShowMessagebox { get; set; }
        public InteractionRequest<Confirmation> ShowConfirmation { get; set; }

        CountryWithFlags _selectedBeerCountry;
        public CountryWithFlags SelectedBeerCountry
        {
            get { return _selectedBeerCountry; }
            set
            {
                _selectedBeerCountry = value;
            }
        }

        private ObservableCollection<BeerViewModel> _beers;
        public ObservableCollection<BeerViewModel> Beers
        {
            get { return _beers; }
            set
            {
                _beers.Clear();
                _beers = value;
                RaisePropertyChanged(() => Beers);
            }
        }

        BeerWithImages _selectedBeer;
        public BeerWithImages SelectedBeer
        {
            get { return _selectedBeer; }
            set
            {
                if (_selectedBeer != value)
                {
                    _selectedBeer = value;
                    RaisePropertyChanged(() => SelectedBeer);
                }
            }
        }

        #endregion


        #region Constructors

        public BeerListViewModel()
        {
            // Commands.
            SelectBeerCommand = new DelegateCommand<BeerViewModel>(OnSelect);

            // Poopup message boxes.
            ShowMessagebox = new InteractionRequest<Notification>();
            ShowConfirmation = new InteractionRequest<Confirmation>();

            // Main list with beers.
            _beers = new ObservableCollection<BeerViewModel>();

            if (!DesignerProperties.IsInDesignTool)
            {
                CompositionInitializer.SatisfyImports(this);

                // When continent is selected call event to get countries from database.
                EventAggregator.GetEvent<BeerCountrySelectedEvent>().Subscribe(OnBeerCountrySelected);

                // When country is added or updated change element in UI. 
                EventAggregator.GetEvent<BeerAddedEvent>().Subscribe(OnBeerAddedOrUpdated);
                EventAggregator.GetEvent<BeerDeletedEvent>().Subscribe(OnBeerDeleted);
            }
        }

        #endregion


        #region Methods to communicate with database

        public void GetCountryBeers(int countryId)
        {
            IsBusy = true;

            var client = new BeerServiceClientWrapper();

            client.GetCountryBeersAsync(countryId);
            client.GetCountryBeersCompleted += delegate (object sender, GetCountryBeersCompletedEventArgs e)
            {
                Beers.Clear();

                if (e.Error == null && e.Result != null)
                {
                    foreach (var beer in e.Result)
                    {
                        var beerWithImages = new BeerWithImages(beer.BeerId);

                        beerWithImages.BeerName = beer.BeerName;
                        beerWithImages.BeerType = beer.BeerType;
                        beerWithImages.CountryId = countryId;

                        beerWithImages.BottleImage.GetThumbnailImage(BitmapCreateOptions.IgnoreImageCache);
                        beerWithImages.CapImage.GetThumbnailImage(BitmapCreateOptions.IgnoreImageCache);
                        beerWithImages.LabelImage.GetThumbnailImage(BitmapCreateOptions.IgnoreImageCache);

                        _beers.Add(new BeerViewModel(beerWithImages));
                    }
                }
                else if (e.Error is FaultException<ExceptionDetail>)
                {
                    var serviceFault = e.Error as FaultException<ExceptionDetail>;
                    
                    ShowMessagebox.Raise(new Notification { Title = "FaultException. Error getting beers.", Content = serviceFault.Detail.Message });
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
                    ShowMessagebox.Raise(new Notification { Title = "Exception. Error getting beers.", Content = e.Error.Message });
                }

                IsBusy = false;
            };
        }

        #endregion


        #region Commands methods

        public void OnBeerCountrySelected(CountryWithFlags beerCountry)
        {
            SelectedBeerCountry = beerCountry;
            GetCountryBeers(beerCountry.CountryId);
        }

        private void OnSelect(BeerViewModel beerViewModel)
        {
            var beer = Beers.FirstOrDefault(x => x == beerViewModel);
            if (beer != null) beer.IsSelected = true;
        }

        #endregion


        #region Events methods
        
        public void OnBeerAddedOrUpdated(BeerWithImages addedBeer)
        {
            if (addedBeer == null) return;

            // Add new beer to the list or update beer title and images in UI.
            if (addedBeer.BeerId == 0)
            {
                GetCountryBeers(addedBeer.CountryId);
            }
            else
            {
                var editingBeer = Beers.FirstOrDefault(b => b.BeerId == addedBeer.BeerId);
                if (editingBeer != null)
                {
                    editingBeer.BeerName = addedBeer.BeerName;
                    editingBeer.BottleImage = addedBeer.BottleImage;
                    editingBeer.CapImage = addedBeer.CapImage;
                    editingBeer.LabelImage = addedBeer.LabelImage;
                }
            }
        }

        public void OnBeerDeleted(int deletedBeer)
        {
            var beerToDelete = _beers.FirstOrDefault(x => x.BeerId == deletedBeer);
            // Remove beer from the list in UI.
            Beers.Remove(beerToDelete);
        }

        #endregion
    }
}
