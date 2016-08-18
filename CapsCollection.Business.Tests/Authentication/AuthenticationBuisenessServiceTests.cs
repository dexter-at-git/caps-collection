using System;
using CapsCollection.Business.BuisenessServices;
using CapsCollection.Business.BuisenessServices.Interfaces;
using CapsCollection.Data.Models;
using CapsCollection.Data.Repositories.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CapsCollection.Business.Tests.Authentication
{
    [TestClass]
    public class AuthenticationBuisenessServiceTests
    {
        private Mock<IUserRepository> _userRepositoryMock = new Mock<IUserRepository>();
        private Mock<IUserSecurityService> _userSecurityServiceMock = new Mock<IUserSecurityService>();
        private AuthenticationBuisenessService _authenticationBuisenessService;

        [TestInitialize]
        public void TestInitialize()
        {
            _authenticationBuisenessService = new AuthenticationBuisenessService(_userRepositoryMock.Object, _userSecurityServiceMock.Object);
        }
        

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AuthenticationBuisenessService_UserNamePaswordNull()
        {
            _authenticationBuisenessService.AuthenticateUser(It.IsAny<string>(), It.IsAny<string>());
            _authenticationBuisenessService.AuthenticateUser("username", It.IsAny<string>());
            _authenticationBuisenessService.AuthenticateUser(It.IsAny<string>(), "password");
        }

        [TestMethod]
        public void AuthenticationBuisenessService_UserName_NotFound()
        {
            var username = "invaliduser";
            var password = "password";
            _userRepositoryMock.Setup(x => x.GetUser(It.Is<string>(u=>u == username), It.IsAny<UserType>())).Returns(It.IsAny<User>());

            var authData = _authenticationBuisenessService.AuthenticateUser(username, password);
            
            Assert.IsNotNull(authData);
            Assert.IsFalse(authData.IsAuthenticated);
            Assert.IsTrue(authData.UserName == username);
        }

        [TestMethod]
        public void AuthenticationBuisenessService_UserName_WrongPassword()
        {
            var username = "user";
            var password = "wongpassword";
            _userRepositoryMock.Setup(x => x.GetUser(It.Is<string>(u => u == username), It.IsAny<UserType>())).Returns(new User() {UserName = username});
            _userSecurityServiceMock.Setup(x=>x.CheckPassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            var authData = _authenticationBuisenessService.AuthenticateUser(username, password);

            Assert.IsNotNull(authData);
            Assert.IsFalse(authData.IsAuthenticated);
            Assert.IsTrue(authData.UserName == username);
        }

        [TestMethod]
        public void AuthenticationBuisenessService_UserName_CorrectPassword()
        {
            var username = "user";
            var password = "password";
            _userRepositoryMock.Setup(x => x.GetUser(It.Is<string>(u => u == username), It.IsAny<UserType>())).Returns(new User() { UserName = username });
            _userSecurityServiceMock.Setup(x => x.CheckPassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            var authData = _authenticationBuisenessService.AuthenticateUser(username, password);

            Assert.IsNotNull(authData);
            Assert.IsTrue(authData.IsAuthenticated);
            Assert.IsTrue(authData.UserName == username);
        }
    }
}
