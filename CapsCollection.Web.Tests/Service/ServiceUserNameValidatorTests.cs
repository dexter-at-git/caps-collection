using System;
using System.ServiceModel;
using CapsCollection.Web.ServiceHost.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CapsCollection.Web.Tests.Service
{
    [TestClass]
    public class ServiceUserNameValidatorTests
    {
        private ServiceUserNameValidator _userNameValidatior;

        [TestInitialize]
        public void TestInitialize()
        {
            _userNameValidatior = new ServiceUserNameValidator();
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceUserNameValidator_EmptyUserNamePassword()
        {
            _userNameValidatior.Validate(It.IsAny<string>(), It.IsAny<string>());
            _userNameValidatior.Validate(It.IsAny<string>(), "password");
            _userNameValidatior.Validate("username", It.IsAny<string>());
        }

        [TestMethod]
        [ExpectedException(typeof(FaultException))]
        public void ServiceUserNameValidator_IncorrectUserNamePassword()
        {
            _userNameValidatior.Validate("username", "password");
        }
    }
}
