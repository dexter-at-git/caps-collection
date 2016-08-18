using CapsCollection.Desktop.Infrastructure.Commands;
using CapsCollection.Desktop.Infrastructure.Interfaces;
using CapsCollection.Desktop.UI.Modules.BulkUpdate.Views;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using CapsCollection.Desktop.UI.Modules.BulkUpdate.Resources;
using Prism.Commands;
using Prism.Events;

namespace CapsCollection.Desktop.UI.Modules.BulkUpdate.ViewModels
{
    public class BulkUpdateViewModel : ViewModelBase, IBulkUpdateViewModel
    {
        #region Private fields

        private readonly IBeerUpdateViewModel _beerUpdateViewModel;
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

        private ObservableCollection<BeerUpdateViewModel> _beerList;
        public ObservableCollection<BeerUpdateViewModel> BeerList
        {
            get { return _beerList; }
            set { base.SetProperty(ref _beerList, value); }
        }

        #endregion


        #region Constructor

        public BulkUpdateViewModel(IBulkUpdateView view, IEventAggregator eventAggregator, IBeerUpdateViewModel beerUpdateViewModel) : base(view)
        {
            _eventAggregator = eventAggregator;
            _beerUpdateViewModel = beerUpdateViewModel;

            // Set tab header
            _headerInfo = String.Format("{0} (0)", BulkUpdateModuleStrings.BulkUpdateBeersTabName);

            // Initialize collections
            _beerList = new ObservableCollection<BeerUpdateViewModel>();

            // Subscribe to events
            _eventAggregator.GetEvent<ImageToUpdateEvent>().Subscribe(OnUpdateBeerRecieved);
            _eventAggregator.GetEvent<BusyEvent>().Subscribe(OnBusyStatusRecieved);
            _eventAggregator.GetEvent<BeerSavedEvent>().Subscribe(OnBeerUpdated);

            // Commands and composite commands
            RefreshCommand = new DelegateCommand(() => { });
            GlobalCommands.RefreshCommand.RegisterCommand(RefreshCommand);
        }

        #endregion


        #region Private methods

        private void OnBeerUpdated(BeerSavedDataEventArgs eventArgs)
        {
            var removeItems = _beerList.Where(x => x.BeerTempId == eventArgs.BeerTempId).ToList();
            removeItems.ForEach(x => _beerList.Remove(x));
            SetTabHeader();
        }
        
        private void OnBusyStatusRecieved(bool isBusy)
        {
            IsBusy = isBusy;
        }
        
        private void OnUpdateBeerRecieved(BeerLoadDataEventArgs obj)
        {
            var beerLoadViewModel = _beerUpdateViewModel.PrepareViewModel(obj);
            if (beerLoadViewModel == null)
            {
                _eventAggregator.GetEvent<BeerErrorEvent>().Publish(new BeerErrorEventArgs() { UserMessage = BulkUpdateModuleStrings.PrepareBeerViewForUpdatingError, Message = BulkUpdateModuleStrings.PrepareBeerViewForUpdatingError });
                return;
            }

            if (!_beerList.Contains(beerLoadViewModel))
            {
                _beerList.Add(beerLoadViewModel);
            }
            SetTabHeader();
        }
        
        private void SetTabHeader()
        {
            _headerInfo = String.Format("{0} ({1})", BulkUpdateModuleStrings.BulkUpdateBeersTabName, _beerList.Count);
            OnPropertyChanged(() => HeaderInfo);
        }

        #endregion
    }
}
