using System;
using CapsCollection.Business.BuisenessServices;
using CapsCollection.Business.BuisenessServices.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CapsCollection.Business.Tests.Authentication
{
    [TestClass]
    public class UserSecurityServiceTests
    {
        private IUserSecurityService _userSecurityService;

        [TestInitialize]
        public void TestInitialize()
        {
            _userSecurityService = new UserSecurityService();
        }
        
        [TestMethod]
        public void UserSecurityService_GenerateSalt()
        {
            var salt = _userSecurityService.GenerateSalt();

            Assert.IsNotNull(salt);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UserSecurityService_CalculatePasswordHash_EmptyParametrs()
        {
            _userSecurityService.CalculatePasswordHash(It.IsAny<string>(), It.IsAny<string>());
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UserSecurityService_CheckPassword_EmptyParametrs()
        {
            _userSecurityService.CalculatePasswordHash(It.IsAny<string>(), It.IsAny<string>());
            _userSecurityService.CheckPassword(new byte[] { }, new byte[] { }, It.IsAny<string>());
        }
        
        [TestMethod]
        public void UserSecurityService_CalculatePasswordHash()
        {
            var password = "password";
            var salt = _userSecurityService.GenerateSalt();

            var passwordHash = _userSecurityService.CalculatePasswordHash(password, salt);

            Assert.IsNotNull(passwordHash);
        }
        
        [TestMethod]
        public void UserSecurityService_CheckPassword_InvalidPassword()
        {
            var password = "password";
            var passwordToCheck = "invalidpassword";
            var saltString = _userSecurityService.GenerateSalt();
            var passwordHashString = _userSecurityService.CalculatePasswordHash(password, saltString);

            var valid = _userSecurityService.CheckPassword(passwordHashString, saltString, passwordToCheck);

            Assert.IsFalse(valid);
        }

        [TestMethod]
        public void UserSecurityService_CheckPassword_ValidPassword()
        {
            var password = "password";
            var passwordToCheck = "password";
            var saltString = _userSecurityService.GenerateSalt();
            var passwordHashString = _userSecurityService.CalculatePasswordHash(password, saltString);

            var valid = _userSecurityService.CheckPassword(passwordHashString, saltString, passwordToCheck);

            Assert.IsTrue(valid);
        }
    }
}
