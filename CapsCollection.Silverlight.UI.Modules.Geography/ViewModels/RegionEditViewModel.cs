using CapsCollection.Silverlight.Infrastructure.Models;
using CapsCollection.Silverlight.Infrastructure.ViewModels;
using CapsCollection.Silverlight.ServiceAgents.Proxies.Geography;
using CapsCollection.Silverlight.UI.Modules.Geography.Validators;
using CapsCollection.Silverlight.UI.Modules.Geography.Views;
using FluentValidation;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Regions;
using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using CapsCollection.Silverlight.Infrastructure.Events;
using CapsCollection.Silverlight.ServiceAgents.ProxiesWrappers;
using CapsCollection.Silverlight.UI.Modules.Services.Interfaces;

namespace CapsCollection.Silverlight.UI.Modules.Geography.ViewModels
{
    public class RegionEditViewModel : ViewModelBase, IConfirmNavigationRequest
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

        public DelegateCommand SaveCommand { get; private set; }
        public DelegateCommand CloseCommand { get; private set; }

        #endregion


        #region Properties and Members

        private readonly RegionModelValidator _validator;

        public InteractionRequest<Notification> ShowMessagebox { get; set; }

        public string Title
        {
            get { return _region != null ? _region.EnglishRegionName : "Empty"; }
        }

        private bool _isDirty;
        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                _isDirty = value;
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        private RegionDto _region;
        public RegionDto Region
        {
            get { return _region; }
            set
            {
                if (_region != value)
                {
                    RaisePropertyChanged(() => Region);
                    RaisePropertyChanged(() => Title);
                }
            }
        }
        
        private string _englishRegionName;
        public string EnglishRegionName
        {
            get { return _englishRegionName; }
            set
            {
                if (_englishRegionName != value)
                {
                    _englishRegionName = value;
                    Region.EnglishRegionName = value;
                    RaisePropertyChanged(() => EnglishRegionName);
                }

                ClearErrorFromProperty("EnglishRegionName");
                var validationResult = _validator.Validate(Region, "EnglishRegionName");
                if (!validationResult.IsValid)
                    validationResult.Errors.ToList().ForEach(x => AddErrorForProperty(x.PropertyName, new ErrorInfo { ErrorMessage = x.ErrorMessage, IsWarning = false }));

                IsDirty = true;
                SaveCommand.RaiseCanExecuteChanged();
            }
        }
        
        private string _nationalRegionName;
        public string NationalRegionName
        {
            get { return _nationalRegionName; }
            set
            {
                if (_nationalRegionName != value)
                {
                    _nationalRegionName = value;
                    Region.NationalRegionName = value;
                    RaisePropertyChanged(() => NationalRegionName);
                }

                ClearErrorFromProperty("NationalRegionName");
                var validationResult = _validator.Validate(Region, "NationalRegionName");
                if (!validationResult.IsValid)
                    validationResult.Errors.ToList().ForEach(x => AddErrorForProperty(x.PropertyName, new ErrorInfo { ErrorMessage = x.ErrorMessage, IsWarning = false }));

                IsDirty = true;
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion


        #region Constructors

        public RegionEditViewModel(RegionModelValidator validator)
        {
            if (validator == null)
                throw new ArgumentNullException("validator");

            _validator = validator;

            // Commands.
            SaveCommand = new DelegateCommand(OnSave, CanSave);
            CloseCommand = new DelegateCommand(OnClose);

            // Poopup message boxes.
            ShowMessagebox = new InteractionRequest<Notification>();

            if (!DesignerProperties.IsInDesignTool)
            {
                CompositionInitializer.SatisfyImports(this);
            }
        }

        #endregion
        

        #region Buttons click command methods

        private bool CanSave()
        {
            IsAuthenticated = AuthenticationManager.AuthenticationInfo.IsAuthenticated;
            return IsDirty && !HasErrors && IsAuthenticated;
        }
        
        private void OnSave()
        {
            IsDirty = false;
            SaveRegion();
        }
        
        private void OnClose()
        {
            IRegion editRegionContentRegion = RegionManager.Regions["PopupRegionContent"];
            foreach (var view in editRegionContentRegion.Views)
            {
                if (view is RegionEditView && ((FrameworkElement)view).DataContext == this)
                {
                    editRegionContentRegion.Remove(view);
                    break;
                }
            }
        }

        #endregion


        #region Methods to communicate with database

        public void GetRegion(int regionId)
        {
            var client = new GeographyServiceClientWrapper();

            client.GetRegionAsync(regionId);
            client.GetRegionCompleted += delegate (object sender, GetRegionCompletedEventArgs e)
            {
                if (e.Error == null)
                {
                    var region = e.Result;

                    _region = region;

                    NationalRegionName = region.NationalRegionName;
                    EnglishRegionName = region.EnglishRegionName;
                }
                else if (e.Error is FaultException<ExceptionDetail>)
                {
                    var serviceFault = e.Error as FaultException<ExceptionDetail>;
                    
                    ShowMessagebox.Raise(new Notification
                    {
                        Title = "FaultException. Error getting region.",
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
                        Title = "Exception. Error getting region.",
                        Content = e.Error.Message
                    }, notification => { });
                }
            };
        }
        
        private void SaveRegion()
        {
            if (HasErrors)
                return;

            var client = new GeographyServiceClientWrapper();

            client.UpdateRegionAsync(Region);
            client.UpdateRegionCompleted += delegate (object sender, AsyncCompletedEventArgs e)
            {
                if (e.Error == null)
                {
                    EventAggregator.GetEvent<RegionAddedEvent>().Publish(Region);
                    OnClose();
                }
                else if (e.Error is FaultException<ExceptionDetail>)
                {
                    var serviceFault = e.Error as FaultException<ExceptionDetail>;

                    var innerException = serviceFault.Detail.InnerException.InnerException.Message;
                    
                    ShowMessagebox.Raise(new Notification
                    {
                        Title = "FaultException. Error saving region.",
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
                        Title = "Exception. Error saving region.",
                        Content = e.Error.Message
                    }, notification => { });
                }
            };
        }

        #endregion
        

        #region INavigationAware

        bool INavigationAware.IsNavigationTarget(NavigationContext navigationContext)
        {
            string regionId = navigationContext.Parameters["countryId"];
            if (!string.IsNullOrWhiteSpace(regionId) && Region != null && Region.RegionId == int.Parse(regionId))
                return true;
            return false;
        }

        void INavigationAware.OnNavigatedTo(NavigationContext navigationContext)
        {
            _region = new RegionDto();

            // Force validate this properties.
            EnglishRegionName = null;
            NationalRegionName = null;

            // Load country based on ID passed in navigation.
            string countryId = navigationContext.Parameters["countryId"];
            if (!string.IsNullOrWhiteSpace(countryId))
            {
                _region.CountryId = int.Parse(countryId);
            }

            // Load region based on ID passed in navigation.
            string regionId = navigationContext.Parameters["regionId"];
            if (!string.IsNullOrWhiteSpace(regionId))
            {
                GetRegion(int.Parse(regionId));
            }
        }
        
        void INavigationAware.OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        #endregion
        

        #region IConfirmNavigationRequest

        void IConfirmNavigationRequest.ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
        {
            if (IsDirty)
            {
                const string prompt = "The view's state has changed and has not been saved, do you want to allow view switching?";
                var result = MessageBox.Show(prompt, "Confirmation", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    continuationCallback(true);
                    return;
                }

                continuationCallback(false);
                return;
            }
            continuationCallback(true);
        }

        #endregion
    }
}
