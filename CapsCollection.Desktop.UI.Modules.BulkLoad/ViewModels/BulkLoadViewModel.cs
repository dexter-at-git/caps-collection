using CapsCollection.Desktop.Infrastructure.Commands;
using CapsCollection.Desktop.Infrastructure.Interfaces;
using CapsCollection.Desktop.UI.Modules.BulkLoad.Views;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using CapsCollection.Desktop.UI.Modules.BulkLoad.Resources;
using Prism.Commands;
using Prism.Events;

namespace CapsCollection.Desktop.UI.Modules.BulkLoad.ViewModels
{
    public class BulkLoadViewModel : ViewModelBase, IBulkLoadViewModel
    {
        #region Private fields

        private readonly IBeerLoadViewModel _beerLoadViewModel;
        private readonly IEventAggregator _eventAggregator;

        #endregion


        #region Commands

        public DelegateCommand RefreshCommand { get; set; }

        #endregion


        #region Properties

        private string _headerInfo;
        public string HeaderInfo
        {
            get { return _headerInfo; }
            set { base.SetProperty(ref _headerInfo, value); }
        }

        private ObservableCollection<BeerLoadViewModel> _beerList;
        public ObservableCollection<BeerLoadViewModel> BeerList
        {
            get { return _beerList; }
            set { base.SetProperty(ref _beerList, value); }
        }

        #endregion


        #region Constructor

        public BulkLoadViewModel(IBulkLoadView view, IEventAggregator eventAggregator, IBeerLoadViewModel beerLoadViewModel)
            : base(view)
        {
            _eventAggregator = eventAggregator;
            _beerLoadViewModel = beerLoadViewModel;

            // Set tab header
            _headerInfo = String.Format("{0} (0)", BulkLoadModuleStrings.BulkLoadBeersTabName);

            // Initialize collections
            _beerList = new ObservableCollection<BeerLoadViewModel>();

            // Subscribe to events
            _eventAggregator.GetEvent<ImageToLoadEvent>().Subscribe(OnImageRecieved);
            _eventAggregator.GetEvent<BusyEvent>().Subscribe(OnBusyStatusRecieved);
            _eventAggregator.GetEvent<BeerSavedEvent>().Subscribe(OnBeerLoaded);
        }

        #endregion


        #region Private methods

        private void OnBeerLoaded(BeerSavedDataEventArgs eventArgs)
        {
            var removeItems = _beerList.Where(x => x.BeerTempId == eventArgs.BeerTempId).ToList();
            removeItems.ForEach(x => _beerList.Remove(x));
            SetTabHeader();
        }

        private void SetTabHeader()
        {
            _headerInfo = String.Format("{0} ({1})", BulkLoadModuleStrings.BulkLoadBeersTabName, _beerList.Count);
            OnPropertyChanged(() => HeaderInfo);
        }

        private void OnBusyStatusRecieved(bool isBusy)
        {
            IsBusy = isBusy;
        }

        private void OnImageRecieved(BeerLoadDataEventArgs imageList)
        {
            var beerLoadViewModel = _beerLoadViewModel.PrepareViewModel(imageList);
            if (beerLoadViewModel == null)
            {
                _eventAggregator.GetEvent<BeerErrorEvent>().Publish(new BeerErrorEventArgs() { UserMessage = BulkLoadModuleStrings.PrepareBeerViewForLoadingError, Message = BulkLoadModuleStrings.PrepareBeerViewForLoadingError });
                return;
            }

            if (!_beerList.Contains(beerLoadViewModel))
            {
                _beerList.Add(beerLoadViewModel);
            }

            SetTabHeader();
        }

        #endregion
    }
}
