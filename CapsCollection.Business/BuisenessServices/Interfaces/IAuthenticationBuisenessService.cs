using CapsCollection.Common.Models;

namespace CapsCollection.Business.BuisenessServices.Interfaces
{
    public interface IAuthenticationBuisenessService
    {
        AuthenticationData AuthenticateUser(string userName, string password);
        AuthenticationData AuthenticateService(string serviceName, string password);
    }
}
