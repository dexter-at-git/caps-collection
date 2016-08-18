using System.Linq;
using CapsCollection.Silverlight.Infrastructure.Models;
using CapsCollection.Silverlight.Infrastructure.ViewModels;
using CapsCollection.Silverlight.ServiceAgents.Proxies.Geography;
using CapsCollection.Silverlight.UI.Modules.Geography.Validators;
using CapsCollection.Silverlight.UI.Modules.Geography.Views;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Regions;
using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows;
using System.ServiceModel;
using CapsCollection.Silverlight.Infrastructure.Events;
using CapsCollection.Silverlight.ServiceAgents.ProxiesWrappers;
using CapsCollection.Silverlight.UI.Modules.Services.Interfaces;
using FluentValidation;

namespace CapsCollection.Silverlight.UI.Modules.Geography.ViewModels
{
    public class CityEditViewModel : ViewModelBase, IConfirmNavigationRequest
    {
        #region MEF import

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

        private readonly CityModelValidator _validator;

        public InteractionRequest<Notification> ShowMessagebox { get; set; }

        public string Title
        {
            get { return _city != null ? _city.EnglishCityName : "Empty"; }
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

        private CityDto _city;
        public CityDto City
        {
            get { return _city; }
            set
            {
                if (_city != value)
                {
                    RaisePropertyChanged(() => City);
                    RaisePropertyChanged(() => Title);
                }
            }
        }

        private string _englishCityName;
        public string EnglishCityName
        {
            get { return _englishCityName; }
            set
            {
                if (_englishCityName != value)
                {
                    _englishCityName = value;
                    City.EnglishCityName = value;
                    RaisePropertyChanged(() => EnglishCityName);
                }

                ClearErrorFromProperty("EnglishCityName");
                var validationResult = _validator.Validate(City, "EnglishCityName");
                if (!validationResult.IsValid)
                    validationResult.Errors.ToList().ForEach(x => AddErrorForProperty(x.PropertyName, new ErrorInfo { ErrorMessage = x.ErrorMessage, IsWarning = false }));

                IsDirty = true;
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        private string _nationalCityName;
        public string NationalCityName
        {
            get { return _nationalCityName; }
            set
            {
                if (_nationalCityName != value)
                {
                    _nationalCityName = value;
                    City.NationalCityName = value;
                    RaisePropertyChanged(() => NationalCityName);
                }

                ClearErrorFromProperty("NationalCityName");
                var validationResult = _validator.Validate(City, "NationalCityName");
                if (!validationResult.IsValid)
                    validationResult.Errors.ToList().ForEach(x => AddErrorForProperty(x.PropertyName, new ErrorInfo { ErrorMessage = x.ErrorMessage, IsWarning = false }));

                IsDirty = true;
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion


        #region Constructors

        public CityEditViewModel(CityModelValidator validator)
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
            SaveCity();
        }

        private void OnClose()
        {
            IRegion editRegionContentRegion = RegionManager.Regions["PopupRegionContent"];
            foreach (var view in editRegionContentRegion.Views)
            {
                if (view is CityEditView && ((FrameworkElement)view).DataContext == this)
                {
                    editRegionContentRegion.Remove(view);
                    break;
                }
            }
        }

        #endregion


        #region Methods to communicate with database

        public void GetCity(int cityId)
        {
            var client = new GeographyServiceClientWrapper();

            client.GetCityAsync(cityId);
            client.GetCityCompleted += delegate (object sender, GetCityCompletedEventArgs e)
            {
                if (e.Error == null)
                {
                    CityDto city = e.Result;

                    _city = city;

                    NationalCityName = city.NationalCityName;
                    EnglishCityName = city.EnglishCityName;
                }
                else if (e.Error is FaultException<ExceptionDetail>)
                {
                    var serviceFault = e.Error as FaultException<ExceptionDetail>;
                    
                    ShowMessagebox.Raise(new Notification
                    {
                        Title = "FaultException. Error getting city.",
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
                        Title = "Exception. Error getting city.",
                        Content = e.Error.Message
                    }, notification => { });
                }
            };
        }

        private void SaveCity()
        {
            if (HasErrors)
                return;

            var client = new GeographyServiceClientWrapper();

            client.UpdateCityAsync(City);
            client.UpdateCityCompleted += delegate (object sender, AsyncCompletedEventArgs e)
            {
                if (e.Error == null)
                {
                    EventAggregator.GetEvent<CityAddedEvent>().Publish(City);
                    OnClose();
                }
                else if (e.Error is FaultException<ExceptionDetail>)
                {
                    var serviceFault = e.Error as FaultException<ExceptionDetail>;

                    var innerException = serviceFault.Detail.InnerException.InnerException.Message;
                    
                    ShowMessagebox.Raise(new Notification
                    {
                        Title = "FaultException. Error saving city.",
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
                        Title = "Exception. Error saving city.",
                        Content = e.Error.Message
                    }, notification => { });
                }
            };
        }

        #endregion


        #region INavigationAware

        bool INavigationAware.IsNavigationTarget(NavigationContext navigationContext)
        {
            string regionId = navigationContext.Parameters["regionId"];
            if (!string.IsNullOrWhiteSpace(regionId) && City != null && City.RegionId == int.Parse(regionId))
                return true;
            return false;
        }

        void INavigationAware.OnNavigatedTo(NavigationContext navigationContext)
        {
            _city = new CityDto();

            // Force validate this properties.
            EnglishCityName = null;
            NationalCityName = null;

            // Load region based on ID passed in.
            string regionId = navigationContext.Parameters["regionId"];
            if (!string.IsNullOrWhiteSpace(regionId))
            {
                _city.RegionId = int.Parse(regionId);
            }

            // Load city based on ID passed in.
            string cityId = navigationContext.Parameters["cityId"];
            if (!string.IsNullOrWhiteSpace(cityId))
            {
                GetCity(int.Parse(cityId));
            }
        }

        void INavigationAware.OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        #endregion


        #region IConfirmNavigationRequest

        void IConfirmNavigationRequest.ConfirmNavigationRequest(NavigationContext navigationContext,
            Action<bool> continuationCallback)
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
