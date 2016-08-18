using System.ServiceModel;
using System.ServiceModel.Web;
using CapsCollection.Common.Models;

namespace CapsCollection.Web.ServiceHost.Contracts
{
    [ServiceContract]
    public interface IAuthenticationService
    {
        [OperationContract]
        [WebInvoke(UriTemplate = "AuthenticateUser", Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped)]
        AuthenticationData AuthenticateUser(string username, string password);
    }
}
