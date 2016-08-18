using System;
using System.IdentityModel.Selectors;
using System.ServiceModel;
using CapsCollection.Business.BuisenessServices;
using CapsCollection.Web.ServiceHost.ServiceBehaviors;

namespace CapsCollection.Web.ServiceHost.Security
{
    public class ServiceUserNameValidator : UserNamePasswordValidator
    {
        public override void Validate(string userName, string password)
        {
            if (String.IsNullOrEmpty(userName) || String.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException();
            }

            var authenticationBuisenessService = (AuthenticationBuisenessService)UnityContainer.Current.Resolve(typeof(AuthenticationBuisenessService), "AuthenticationBuisenessService");

            var serviceAuthenticationData = authenticationBuisenessService.AuthenticateService(userName, password);

            if (!serviceAuthenticationData.IsAuthenticated)
            {
                throw new FaultException("Unknown servicename or incorrect password");
            }
        }
    }
}