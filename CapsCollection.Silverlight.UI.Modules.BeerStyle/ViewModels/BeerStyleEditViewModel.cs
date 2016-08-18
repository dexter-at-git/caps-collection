using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using CapsCollection.Silverlight.Infrastructure.Events;
using CapsCollection.Silverlight.Infrastructure.Models;
using CapsCollection.Silverlight.Infrastructure.ViewModels;
using CapsCollection.Silverlight.ServiceAgents.Proxies.Beer;
using CapsCollection.Silverlight.ServiceAgents.ProxiesWrappers;
using CapsCollection.Silverlight.UI.Modules.BeerStyle.Validators;
using CapsCollection.Silverlight.UI.Modules.BeerStyle.Views;
using CapsCollection.Silverlight.UI.Modules.Services.Interfaces;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Regions;
using FluentValidation;

namespace CapsCollection.Silverlight.UI.Modules.BeerStyle.ViewModels
{
    public class BeerStyleEditViewModel : ViewModelBase, INavigationAware
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

        private readonly BeerStyleModelValidator _validator;

        public InteractionRequest<Notification> ShowMessagebox { get; set; }


        public string Title
        {
            get { return _beerStyle != null ? _beerStyle.BeerStyleName : "Empty"; }
        }

        bool _isDirty;
        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                _isDirty = value;
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        BeerStyleDto _beerStyle;
        public BeerStyleDto BeerStyle
        {
            get { return _beerStyle; }
            set
            {
                if (_beerStyle != value)
                {
                    RaisePropertyChanged(() => BeerStyle);
                    RaisePropertyChanged(() => Title);
                }
            }
        }

        private string _beerStyleName;
        public string BeerStyleName
        {
            get { return _beerStyleName; }
            set
            {
                if (_beerStyleName != value)
                {
                    _beerStyleName = value;
                    BeerStyle.BeerStyleName = value;
                    RaisePropertyChanged(() => BeerStyleName);
                }

                ClearErrorFromProperty("BeerStyleName");
                var validationResult = _validator.Validate(BeerStyle, "BeerStyleName");
                if (!validationResult.IsValid)
                    validationResult.Errors.ToList().ForEach(x => AddErrorForProperty(x.PropertyName, new ErrorInfo { ErrorMessage = x.ErrorMessage, IsWarning = false }));

                IsDirty = true;
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion


        #region Constructors

        public BeerStyleEditViewModel(BeerStyleModelValidator validator)
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
            SaveBeerStyle();
        }

        private void OnClose()
        {
            IRegion editBeerStyleContentRegion = RegionManager.Regions["PopupRegionContent"];
            foreach (var view in editBeerStyleContentRegion.Views)
            {
                if (view is BeerStyleEditView && ((FrameworkElement)view).DataContext == this)
                {
                    editBeerStyleContentRegion.Remove(view);
                    break;
                }
            }
        }

        #endregion


        #region Methods to communicate with database

        public void GetBeerStyle(int beerStyleId)
        {
            var client = new BeerServiceClientWrapper();

            client.GetBeerStyleAsync(beerStyleId);
            client.GetBeerStyleCompleted += delegate (object sender, GetBeerStyleCompletedEventArgs e)
            {
                if (e.Error == null)
                {
                    var beerStyle = e.Result;

                    _beerStyle = beerStyle;

                    BeerStyleName = beerStyle.BeerStyleName;

                    IsDirty = false;
                }
                else if (e.Error is FaultException<ExceptionDetail>)
                {
                    var serviceFault = e.Error as FaultException<ExceptionDetail>;
                    
                    ShowMessagebox.Raise(new Notification
                    {
                        Title = "FaultException. Error loading beer style.",
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
                        Title = "Exception. Error loading beer style.",
                        Content = e.Error.Message
                    }, notification => { });
                }
            };
        }

        private void SaveBeerStyle()
        {
            if (HasErrors)
                return;

            var client = new BeerServiceClientWrapper();

            client.UpdateBeerStyleAsync(BeerStyle);
            client.UpdateBeerStyleCompleted += delegate (object sender, AsyncCompletedEventArgs e)
            {
                if (e.Error == null)
                {
                    EventAggregator.GetEvent<BeerStyleAddedEvent>().Publish(BeerStyle);
                    OnClose();
                }
                else if (e.Error is FaultException<ExceptionDetail>)
                {
                    var serviceFault = e.Error as FaultException<ExceptionDetail>;

                    var innerException = serviceFault.Detail.InnerException.InnerException.Message;
                    
                    ShowMessagebox.Raise(new Notification
                    {
                        Title = "FaultException. Error saving beer style.",
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
                        Title = "Exception. Error saving beer style.",
                        Content = e.Error.Message
                    }, notification => { });
                }
            };
        }

        #endregion


        #region INavigationAware

        bool INavigationAware.IsNavigationTarget(NavigationContext navigationContext)
        {
            string beerStyleId = navigationContext.Parameters["beerStyleId"];
            if (!string.IsNullOrWhiteSpace(beerStyleId) && BeerStyle != null && BeerStyle.BeerStyleId == int.Parse(beerStyleId))
                return true;
            return false;
        }

        void INavigationAware.OnNavigatedTo(NavigationContext navigationContext)
        {
            _beerStyle = new BeerStyleDto();

            // Force validate this fields.
            BeerStyleName = string.Empty;

            // Load beer style based on ID passed in navigation parameters.
            string beerStyleId = navigationContext.Parameters["beerStyleId"];
            if (!string.IsNullOrWhiteSpace(beerStyleId))
            {
                GetBeerStyle(int.Parse(beerStyleId));
            }

        }

        void INavigationAware.OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        #endregion
    }
}
