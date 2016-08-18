using CapsCollection.Silverlight.ServiceAgents.Proxies.Authentication;

namespace CapsCollection.Silverlight.UI.Modules.Services.Interfaces
{
    public interface IAuthenticationManager
    {
        AuthenticationData AuthenticationInfo { get; }
        void Authenticate(string userName, string password);
        void Logout();
    }
}
