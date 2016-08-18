using CapsCollection.Common.Settings;
using CapsCollection.Silverlight.ServiceAgents.Proxies.Beer;

namespace CapsCollection.Silverlight.ServiceAgents.ProxiesWrappers
{
    public class BeerServiceClientWrapper : BeerServiceClient
    {
        public BeerServiceClientWrapper()
            : base(CapsCollectionEndpointSettings.HttpBinding,
                CapsCollectionEndpointSettings.BeerServiceEndpointAddress)
        {
            this.ClientCredentials.UserName.UserName = CapsCollectionEndpointSettings.ServiceCredentials.UserName;
            this.ClientCredentials.UserName.Password = CapsCollectionEndpointSettings.ServiceCredentials.Password;
        }
    }
}