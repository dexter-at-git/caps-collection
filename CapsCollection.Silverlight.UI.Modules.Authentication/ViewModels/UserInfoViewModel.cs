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
    public class UserInfoViewModel : ViewModelBase
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

        public DelegateCommand LogoutCommand { get; private set; }

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
                _userName = value;
                RaisePropertyChanged("UserName");
            }
        }

        private string _userStatusText;
        public string UserStatusText
        {
            get
            {
                return _userStatusText;
            }
            set
            {
                _userStatusText = value;
                RaisePropertyChanged("UserStatusText");
            }
        }

        #endregion


        #region Constructors

        public UserInfoViewModel()
        {
            // Commands.
            LogoutCommand = new DelegateCommand(OnLogout, CanLogout);

            if (!DesignerProperties.IsInDesignTool)
            {
                CompositionInitializer.SatisfyImports(this);

                EventAggregator.GetEvent<IsAuthenticatedEvent>().Subscribe(CheckUserStatus);
            }

            CheckUserStatus(AuthenticationManager.AuthenticationInfo);
        }


        #endregion


        #region Event methods

        public void CheckUserStatus(AuthenticationData authenticationData)
        {
            IsAuthenticated = authenticationData.IsAuthenticated;

            if (authenticationData.IsAuthenticated)
            {
                UserName = authenticationData.UserName;
                UserStatusText = "You have full access.";
                return;
            }

            if (!String.IsNullOrEmpty(authenticationData.UserName))
            {
                UserName = authenticationData.UserName;
                UserStatusText = authenticationData.ErrorMessage + " You still have read-only access and you can't save anything.";
                return;
            }

            UserName = "Guest";
            UserStatusText = "You have read-only access and you can't save anything.";
        }

        #endregion


        #region Buttons click command methods

        public void OnLogout()
        {
            // Log out current user.
            AuthenticationManager.Logout();

            // Publish authentication status.
            EventAggregator.GetEvent<IsAuthenticatedEvent>().Publish(AuthenticationManager.AuthenticationInfo);
        }

        public bool CanLogout()
        {
            return true;
        }

        #endregion
    }
}
