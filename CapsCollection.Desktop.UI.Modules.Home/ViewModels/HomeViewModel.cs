using System;
using System.Linq;
using System.Runtime.CompilerServices;
using CapsCollection.Desktop.Infrastructure.Commands;
using CapsCollection.Desktop.Infrastructure.Interfaces;
using CapsCollection.Desktop.Infrastructure.Models;
using CapsCollection.Desktop.UI.Modules.Home.Models;
using CapsCollection.Desktop.UI.Modules.Home.Resources;
using CapsCollection.Desktop.UI.Modules.Home.Views;
using FluentValidation;
using Prism.Commands;
using Prism.Events;

namespace CapsCollection.Desktop.UI.Modules.Home.ViewModels
{
    public class HomeViewModel : ViewModelBase, IHomeViewModel
    {
        #region Private properties

        private readonly IEventAggregator _eventAggregator;
        private readonly IImageTypeAggregator _imageTypeAggregator;
        private readonly IValidator<HomeViewModel> _validator;

        #endregion


        #region Commands

        public DelegateCommand ProcessImagesCommand { get; private set; }

        #endregion


        #region Properties

        public string HeaderInfo
        {
            get { return HomeModuleStrings.HomeTabName; }
        }

        private ImageTypeStatistics _imageTypeStatistics;
        public ImageTypeStatistics ImageTypeStatistics
        {
            get { return _imageTypeStatistics; }
            set
            {
                base.SetProperty(ref _imageTypeStatistics, value);
            }
        }

        private string _bottlesLookupPath;
        public string BottlesLookupPath
        {
            get { return _bottlesLookupPath; }
            set
            {
                base.SetProperty(ref _bottlesLookupPath, value);
                ValidateProperty();
                SearchLookupPath(value, ImageType.Bottle);
            }
        }

        private string _capsLookupPath;
        public string CapsLookupPath
        {
            get { return _capsLookupPath; }
            set
            {
                base.SetProperty(ref _capsLookupPath, value);
                ValidateProperty();
                SearchLookupPath(value, ImageType.Cap);
            }
        }

        private string _labelsLookupPath;
        public string LabelsLookupPath
        {
            get { return _labelsLookupPath; }
            set
            {
                base.SetProperty(ref _labelsLookupPath, value);
                ValidateProperty();
                SearchLookupPath(value, ImageType.Label);
            }
        }

        private bool _checkUpdates;
        public bool CheckUpdates
        {
            get { return _checkUpdates; }
            set { base.SetProperty(ref _checkUpdates, value); }
        }

        #endregion


        #region Constructor

        public HomeViewModel(IHomeView view, IEventAggregator eventAggregator, IValidatorFactory validatorFactory, IImageTypeAggregator imageTypeAggregator)
            : base(view)
        {
            _eventAggregator = eventAggregator;
            _imageTypeAggregator = imageTypeAggregator;
            _validator = validatorFactory.GetValidator<HomeViewModel>();

            // Initialize objects
            _imageTypeStatistics = new ImageTypeStatistics();

            // Commands
            ProcessImagesCommand = new DelegateCommand(OnProcess, CanProcess);

            // Subscribe to events
            _eventAggregator.GetEvent<BusyEvent>().Subscribe(OnBusyStatusRecieved);

            // Initialize test data
            InitialzeTestData();
        }

        #endregion


        #region Button commands

        public void InitialzeTestData()
        {
            BottlesLookupPath = @"C:\Users\g.fil\Downloads\1. Italy\bottles";
            CapsLookupPath = @"C:\Users\g.fil\Downloads\1. Italy\caps";
            LabelsLookupPath = @"C:\Users\g.fil\Downloads\1. Italy\labels";
        }

        #endregion


        #region Buttons click command methods

        private bool CanProcess()
        {
            if (String.IsNullOrEmpty(_bottlesLookupPath) && String.IsNullOrEmpty(_capsLookupPath) && String.IsNullOrEmpty(_labelsLookupPath))
            {
                return false;
            }
            return !HasErrors;
        }
        
        private void OnProcess()
        {
            var combinedImages = _imageTypeAggregator.CombineImages();

            var imageProcessingData = new ImageProcessingDataEventArgs()
            {
                CheckUpdates = _checkUpdates,
                CombinedImages = combinedImages
            };

            _eventAggregator.GetEvent<ImagesProcessingEvent>().Publish(imageProcessingData);
        }

        #endregion


        #region Private methods

        private void OnBusyStatusRecieved(bool isBusy)
        {
            IsBusy = isBusy;
        }

        private void SearchLookupPath(string lookupPath, ImageType imageType)
        {
            if (!HasErrors)
            {
                _imageTypeAggregator.PutImageType(lookupPath, imageType);
            }
            else
            {
                _imageTypeAggregator.RemoveImageType(imageType);
            }
            ImageTypeStatistics = _imageTypeAggregator.GetImageTypeStaticstics();
            OnPropertyChanged(() => ImageTypeStatistics);
            ProcessImagesCommand.RaiseCanExecuteChanged();
        }

        private void ValidateProperty([CallerMemberName]string propertyName = "")
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                return;
            }
            ClearErrorFromProperty(propertyName);
            var validationResult = _validator.Validate(this, propertyName);
            if (!validationResult.IsValid)
            {
                validationResult.Errors.ToList().ForEach(x => AddErrorForProperty(x.PropertyName, new ErrorInfo { ErrorMessage = x.ErrorMessage, IsWarning = false }));
            }
        }

        #endregion
    }
}
