using System;
using CapsCollection.Silverlight.ServiceAgents.Proxies.Authentication;
using System.ComponentModel.Composition;
using System.ServiceModel;
using CapsCollection.Silverlight.Infrastructure.Events;
using CapsCollection.Silverlight.ServiceAgents.ProxiesWrappers;
using CapsCollection.Silverlight.UI.Modules.Services.Interfaces;
using Microsoft.Practices.Prism.Events;

namespace CapsCollection.Silverlight.UI.Modules.Services
{
    [Export(typeof(IAuthenticationManager))]
    public class AuthenticationManager : IAuthenticationManager
    {
        #region MEF imports

        [Import]
        public IEventAggregator EventAggregator { get; set; }

        #endregion


        #region Properties and Members
        
        public AuthenticationData AuthenticationInfo { get; set; }

        #endregion


        #region Constructor

        public AuthenticationManager()
        {
            AuthenticationInfo = new AuthenticationData();
        }

        #endregion


        #region Interface implementation

        public void Authenticate(string userName, string password)
        {
            var client = new AuthenticationServiceClientWrapper();

            client.AuthenticateUserAsync(userName, password);
            client.AuthenticateUserCompleted += delegate (object sender, AuthenticateUserCompletedEventArgs e)
            {
                if (e.Error == null && e.Result != null)
                {
                    AuthenticationInfo = e.Result;

                }
                else if (e.Error is FaultException<ExceptionDetail>)
                {
                    var serviceFault = e.Error as FaultException<ExceptionDetail>;
                    AuthenticationInfo.IsAuthenticated = false;
                    AuthenticationInfo.ErrorMessage = serviceFault.Detail.Message;
                }
                else if (e.Error != null)
                {
                    AuthenticationInfo.IsAuthenticated = false;
                    AuthenticationInfo.ErrorMessage = e.Error.Message;
                }

                // Publish authentication status.
                EventAggregator.GetEvent<IsAuthenticatedEvent>().Publish(AuthenticationInfo);
            };
        }
        
        public void Logout()
        {
            AuthenticationInfo = new AuthenticationData()
            {
                UserName = String.Empty,
                IsAuthenticated = false,
                ErrorMessage = String.Empty
            };
        }

        #endregion
    }
}
