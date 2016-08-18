using CapsCollection.Common.Settings;
using CapsCollection.Silverlight.ServiceAgents.Proxies.Authentication;

namespace CapsCollection.Silverlight.ServiceAgents.ProxiesWrappers
{
    public class AuthenticationServiceClientWrapper : AuthenticationServiceClient
    {
        public AuthenticationServiceClientWrapper()
            : base(CapsCollectionEndpointSettings.HttpBinding,
                CapsCollectionEndpointSettings.AuthenticationServiceEndpointAddress)
        {
            this.ClientCredentials.UserName.UserName = CapsCollectionEndpointSettings.ServiceCredentials.UserName;
            this.ClientCredentials.UserName.Password = CapsCollectionEndpointSettings.ServiceCredentials.Password;
        }
    }
}
