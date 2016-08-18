using System;
using System.ServiceModel;
using CapsCollection.Web.ServiceHost.Contracts;
using CapsCollection.Web.ServiceHost.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CapsCollection.Web.Tests.Service
{
    [TestClass]
    public class BeerServiceTests
    {
        private static System.ServiceModel.ServiceHost _serviceHost;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            // If there is an error "HTTP could not register URL", then run VS in Administrator mode.
            _serviceHost = new System.ServiceModel.ServiceHost(typeof(BeerService), new[] { new Uri("http://127.0.0.1:8000") });
            _serviceHost.AddServiceEndpoint(typeof(IBeerService), new BasicHttpBinding(), "BeerService");
            _serviceHost.Open();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _serviceHost.Close();
        }


        [TestMethod]
        [Ignore]
        public void BeerService_ShouldNotFail()
        {
            var endpoint = new EndpointAddress("http://127.0.0.1:8000/BeerService");
            var proxy = ChannelFactory<IBeerService>.CreateChannel(new BasicHttpBinding(), endpoint);
            var data = proxy.GetBeer(1);
        }
    }
}
