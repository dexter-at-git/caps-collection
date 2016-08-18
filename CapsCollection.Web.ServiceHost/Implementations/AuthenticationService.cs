using System;
using System.ServiceModel;
using CapsCollection.Business.BuisenessServices.Interfaces;
using CapsCollection.Common.Models;
using CapsCollection.Web.ServiceHost.Contracts;
using CapsCollection.Web.ServiceHost.ServiceBehaviors;

namespace CapsCollection.Web.ServiceHost.Implementations
{
    [UnityInstanceProviderServiceBehavior()]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class AuthenticationService : IAuthenticationService
    {
        #region Members

        private readonly IAuthenticationBuisenessService _authenticationBuisenessService;

        #endregion


        #region Constructor

        public AuthenticationService(IAuthenticationBuisenessService authenticationBuisenessService)
        {
            if (authenticationBuisenessService == null)
                throw new ArgumentNullException("authenticationBuisenessService");

            _authenticationBuisenessService = authenticationBuisenessService;
        }


        #endregion


        #region Implementation

        public AuthenticationData AuthenticateUser(string username, string password)
        {
            var authenticationData = _authenticationBuisenessService.AuthenticateUser(username, password);

            return authenticationData;
        }

        #endregion
    }
}
