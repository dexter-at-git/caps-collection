using System;
using System.ServiceModel;
using CapsCollection.Web.ServiceHost.Contracts;
using CapsCollection.Web.ServiceHost.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CapsCollection.Web.Tests.Service
{
    [TestClass]
    public class AuthenticationServiceTests
    {
        private static System.ServiceModel.ServiceHost _serviceHost;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            // If there is an error "HTTP could not register URL", then run VS in Administrator mode.
            _serviceHost = new System.ServiceModel.ServiceHost(typeof(AuthenticationService), new[] { new Uri("http://127.0.0.1:8000") });
            _serviceHost.AddServiceEndpoint(typeof(IAuthenticationService), new BasicHttpBinding(), "AuthenticationService");
            _serviceHost.Open();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _serviceHost.Close();
        }


        [TestMethod]
        [Ignore]
        public void AuthenticationService_ShouldNotFail()
        {
            var endpoint = new EndpointAddress("http://127.0.0.1:8000/AuthenticationService");
            var proxy = ChannelFactory<IAuthenticationService>.CreateChannel(new BasicHttpBinding(), endpoint);
            var data = proxy.AuthenticateUser("test", "test");
        }
    }
}
