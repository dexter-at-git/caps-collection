using CapsCollection.Common.Settings;
using CapsCollection.Silverlight.ServiceAgents.Proxies.Geography;

namespace CapsCollection.Silverlight.ServiceAgents.ProxiesWrappers
{
    public class GeographyServiceClientWrapper : GeographyServiceClient
    {
        public GeographyServiceClientWrapper()
            : base(CapsCollectionEndpointSettings.HttpBinding,
                CapsCollectionEndpointSettings.GeographyServiceEndpointAddress)
        {
            this.ClientCredentials.UserName.UserName = CapsCollectionEndpointSettings.ServiceCredentials.UserName;
            this.ClientCredentials.UserName.Password = CapsCollectionEndpointSettings.ServiceCredentials.Password;
        }
    }
}