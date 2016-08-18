using System;
using CapsCollection.Silverlight.Infrastructure.ViewModels;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using System.ComponentModel;
using System.ComponentModel.Composition;
using CapsCollection.Silverlight.Infrastructure.Events;
using CapsCollection.Silverlight.ServiceAgents.Proxies.Authentication;
using CapsCollection.Silverlight.UI.Modules.Services.Interfaces;

namespace CapsCollection.Silverlight.UI.Modules.Authentication.ViewModels
{
    public class LoginViewModel : ViewModelBase
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

        public DelegateCommand LoginCommand { get; private set; }

        #endregion


        #region Properties and Members

        private string _userName;
        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                if (_userName != value)
                {
                    _userName = value;
                    RaisePropertyChanged(() => UserName);
                }
                _userName = value;

                LoginCommand.RaiseCanExecuteChanged();
            }
        }
        
        private string _password;
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                if (_password != value)
                {
                    _password = value;
                    RaisePropertyChanged(() => Password);
                }

                LoginCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion


        #region Constructors

        public LoginViewModel()
        {
            LoginCommand = new DelegateCommand(OnLogin, CanLogin);
            
            if (!DesignerProperties.IsInDesignTool)
            {
                CompositionInitializer.SatisfyImports(this);

                EventAggregator.GetEvent<IsAuthenticatedEvent>().Subscribe(OnAuthenticationCompleted);
            }
        }

        #endregion

        
        #region Buttons click command methods

        public void OnLogin()
        {
            IsBusy = true;
            
            AuthenticationManager.Authenticate(UserName, Password);
        }

        public bool CanLogin()
        {
            if (String.IsNullOrEmpty(UserName) || String.IsNullOrEmpty(Password))
            {
                return false;
            }
            return true;
        }

        #endregion


        #region Methods to manipulate elements in list in UI
        
        public void OnAuthenticationCompleted(AuthenticationData authenticationData)
        {
            UserName = String.Empty;
            Password = String.Empty;

            IsBusy = false;
        }

        #endregion
    }
}
