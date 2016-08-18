using CapsCollection.Silverlight.Infrastructure.Models;
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
using System.ServiceModel;
using System.Windows.Media.Imaging;
using CapsCollection.Silverlight.Infrastructure.Events;
using CapsCollection.Silverlight.ServiceAgents.Proxies.Geography;
using CapsCollection.Silverlight.ServiceAgents.ProxiesWrappers;
using CapsCollection.Silverlight.UI.Modules.Services.Interfaces;
using FileOperation = CapsCollection.Silverlight.ServiceAgents.Proxies.Beer.FileOperation;
using ImageFileOperationDto = CapsCollection.Silverlight.ServiceAgents.Proxies.Beer.ImageFileOperationDto;

namespace CapsCollection.Silverlight.UI.Modules.Collection.ViewModels
{
    public class BeerViewModel : ViewModelBase
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

        public DelegateCommand EditBeerCommand { get; private set; }
        public DelegateCommand DeleteBeerCommand { get; private set; }
        public DelegateCommand<object> SelectBeerCommand { get; private set; }

        public DelegateCommand GetBottlePreviewCommand { get; private set; }
        public DelegateCommand GetCapPreviewCommand { get; private set; }
        public DelegateCommand GetLabelPreviewCommand { get; private set; }

        #endregion


        #region Properties and Members

        public InteractionRequest<Notification> ShowMessagebox { get; set; }
        public InteractionRequest<Confirmation> ShowConfirmation { get; set; }
        
        BeerCountryDto _selectedBeerCountry;
        public BeerCountryDto SelectedBeerCountry
        {
            get { return _selectedBeerCountry; }
            set
            {
                _selectedBeerCountry = value;
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
                    EditBeerCommand.RaiseCanExecuteChanged();
                    DeleteBeerCommand.RaiseCanExecuteChanged();
                }
            }
        }
        
        private int _beerId;
        public int BeerId
        {
            get
            {
                return _beerId;
            }
            set
            {
                _beerId = value;
                RaisePropertyChanged("BeerId");
            }
        }
        
        private string _beerName;
        public string BeerName
        {
            get
            {
                return _beerName;
            }
            set
            {
                _beerName = value;
                RaisePropertyChanged("BeerName");
            }
        }
        
        private string _beerType;
        public string BeerType
        {
            get
            {
                return _beerType;
            }
            set
            {
                _beerType = value;
                RaisePropertyChanged("BeerType");
            }
        }

        BeerImage _bottleImage;
        public BeerImage BottleImage
        {
            get { return _bottleImage; }
            set
            {
                _bottleImage = value;
                RaisePropertyChanged("BottleImage");
            }
        }
        
        BeerImage _capImage;
        public BeerImage CapImage
        {
            get { return _capImage; }
            set
            {
                _capImage = value;
                RaisePropertyChanged("CapImage");
            }
        }
        
        private BeerImage _labelImage;
        public BeerImage LabelImage
        {
            get { return _labelImage; }
            set
            {
                _labelImage = value;
                RaisePropertyChanged("LabelImage");
            }
        }

        bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                RaisePropertyChanged(() => IsSelected);
                EditBeerCommand.RaiseCanExecuteChanged();
                DeleteBeerCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion


        #region Constructors

        public BeerViewModel(BeerWithImages beer)
        {
            // Commands.
            EditBeerCommand = new DelegateCommand(OnEditBeer, CanEditBeer);
            DeleteBeerCommand = new DelegateCommand(OnDeleteBeer, CanDeleteBeer);
            SelectBeerCommand = new DelegateCommand<object>(OnMouseEnter);
            GetBottlePreviewCommand = new DelegateCommand(OnGetBottlePreview);
            GetCapPreviewCommand = new DelegateCommand(OnGetCapPreview);
            GetLabelPreviewCommand = new DelegateCommand(OnGetLabelPreview);

            _beerId = beer.BeerId;
            _beerName = beer.BeerName;
            _beerType = beer.BeerType;
            _bottleImage = beer.BottleImage;
            _capImage = beer.CapImage;
            _labelImage = beer.LabelImage;

            _selectedBeer = beer;

            // Poopup message boxes.
            ShowMessagebox = new InteractionRequest<Notification>();
            ShowConfirmation = new InteractionRequest<Confirmation>();


            if (!DesignerProperties.IsInDesignTool)
            {
                CompositionInitializer.SatisfyImports(this);
            }
        }

        #endregion


        #region Methods to communicate with database

        public void DeleteBeer()
        {
            IsBusy = true;

            var client = new BeerServiceClientWrapper();

            BeerDto beer = new BeerDto();
            beer.BeerId = _beerId;

            client.DeleteBeerAsync(beer);
            client.DeleteBeerCompleted += delegate (object sender, AsyncCompletedEventArgs e)
            {
                if (e.Error == null)
                {
                    var imageFilesToDelete = FormImageFilessToDelete(_selectedBeer);
                    ProcessImageFiles(imageFilesToDelete);

                    EventAggregator.GetEvent<BeerDeletedEvent>().Publish(_beerId);
                }
                else if (e.Error is FaultException<ExceptionDetail>)
                {
                    FaultException<ExceptionDetail> serviceFault = e.Error as FaultException<ExceptionDetail>;
                    
                    ShowMessagebox.Raise(new Notification { Title = "FaultException. Error deleting beer.", Content = serviceFault.Detail.Message });
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
                    ShowMessagebox.Raise(new Notification { Title = "Exception. Error deleting beer.", Content = e.Error.Message });
                }

                IsBusy = false;
            };
        }


        private void ProcessImageFiles(ObservableCollection<ImageFileOperationDto> imageFiles)
        {
            var client = new BeerServiceClientWrapper();
            client.ProcessImageFilesAsync(imageFiles);

            IsBusy = true;

            client.ProcessImageFilesCompleted += delegate (object sender, AsyncCompletedEventArgs e)
            {
                IsBusy = false;

                if (e.Error == null)
                {
                }
                else if (e.Error is FaultException<ExceptionDetail>)
                {
                    var serviceFault = e.Error as FaultException<ExceptionDetail>;

                    var innerException = serviceFault.Detail.InnerException;
                    
                    ShowMessagebox.Raise(new Notification { Title = "FaultException. Error during file operations.", Content = serviceFault.Detail.Message + Environment.NewLine + innerException });
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
                    ShowMessagebox.Raise(new Notification { Title = "Exception. Error during file operations.", Content = e.Error.Message });
                }
            };
        }

        #endregion


        #region Buttons click command methods

        private void OnMouseEnter(object obj)
        {
            IsSelected = Boolean.Parse(obj.ToString());
        }

        private void OnEditBeer()
        {
            var uriQuery = new UriQuery();
            uriQuery.Add("beerId", _beerId.ToString(CultureInfo.InvariantCulture));

            var uri = new Uri("BeerEditView" + uriQuery, UriKind.Relative);
            RegionManager.RequestNavigate("BeerEditContent", uri);

            EventAggregator.GetEvent<ShowBeerEditRegionEvent>().Publish(true);
        }

        private bool CanEditBeer()
        {
            return IsSelected;
        }

        private void OnDeleteBeer()
        {
            ShowConfirmation.Raise(new Confirmation { Content = "Selected beer will be deleted. Do you accept?", Title = "Delete confirmation" }, confirmation =>
            {
                if (confirmation.Confirmed)
                {
                    DeleteBeer();
                    EventAggregator.GetEvent<CountryBeerDeletedEvent>().Publish(_selectedBeer.CountryId);
                }
            });
        }

        private bool CanDeleteBeer()
        {
            IsAuthenticated = AuthenticationManager.AuthenticationInfo.IsAuthenticated;

            return IsSelected && IsAuthenticated;
        }

        private void OnGetBottlePreview()
        {
            _bottleImage.GetImagePreview(BitmapCreateOptions.IgnoreImageCache);

        }

        private void OnGetCapPreview()
        {
            // Load preview cap image.
            _capImage.GetImagePreview(BitmapCreateOptions.IgnoreImageCache);

        }

        private void OnGetLabelPreview()
        {
            // Load preview label image.
            _labelImage.GetImagePreview(BitmapCreateOptions.IgnoreImageCache);
        }

        #endregion


        #region Private methods

        private ObservableCollection<ImageFileOperationDto> FormImageFilessToDelete(BeerWithImages beer)
        {
            var listImageFiles = new ObservableCollection<ImageFileOperationDto>();

            listImageFiles.Add(new ImageFileOperationDto
            {
                ImageBytes = null,
                Container = beer.BottleImage.ImageContainerPath,
                FileName = beer.BottleImage.ImageHiResFileName,
                FileOperation = FileOperation.Delete
            });

            listImageFiles.Add(new ImageFileOperationDto
            {
                ImageBytes = null,
                Container = beer.BottleImage.ImageContainerPath,
                FileName = beer.BottleImage.ImagePreviewFileName,
                FileOperation = FileOperation.Delete
            });

            listImageFiles.Add(new ImageFileOperationDto
            {
                ImageBytes = null,
                Container = beer.BottleImage.ImageContainerPath,
                FileName = beer.BottleImage.ImageThumbnailFileName,
                FileOperation = FileOperation.Delete
            });

            listImageFiles.Add(new ImageFileOperationDto
            {
                ImageBytes = null,
                Container = beer.CapImage.ImageContainerPath,
                FileName = beer.CapImage.ImageHiResFileName,
                FileOperation = FileOperation.Delete
            });

            listImageFiles.Add(new ImageFileOperationDto
            {
                ImageBytes = null,
                Container = beer.CapImage.ImageContainerPath,
                FileName = beer.CapImage.ImagePreviewFileName,
                FileOperation = FileOperation.Delete
            });

            listImageFiles.Add(new ImageFileOperationDto
            {
                ImageBytes = null,
                Container = beer.CapImage.ImageContainerPath,
                FileName = beer.CapImage.ImageThumbnailFileName,
                FileOperation = FileOperation.Delete
            });

            listImageFiles.Add(new ImageFileOperationDto
            {
                ImageBytes = null,
                Container = beer.LabelImage.ImageContainerPath,
                FileName = beer.LabelImage.ImageHiResFileName,
                FileOperation = FileOperation.Delete
            });

            listImageFiles.Add(new ImageFileOperationDto
            {
                ImageBytes = null,
                Container = beer.LabelImage.ImageContainerPath,
                FileName = beer.LabelImage.ImagePreviewFileName,
                FileOperation = FileOperation.Delete
            });

            listImageFiles.Add(new ImageFileOperationDto
            {
                ImageBytes = null,
                Container = beer.LabelImage.ImageContainerPath,
                FileName = beer.LabelImage.ImageThumbnailFileName,
                FileOperation = FileOperation.Delete
            });

            return listImageFiles;
        }

        #endregion
    }
}
