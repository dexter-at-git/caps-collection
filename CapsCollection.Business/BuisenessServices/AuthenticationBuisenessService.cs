using System;
using CapsCollection.Business.BuisenessServices.Interfaces;
using CapsCollection.Common.Models;
using CapsCollection.Data.Models;
using CapsCollection.Data.Repositories.Interfaces;

namespace CapsCollection.Business.BuisenessServices
{
    public class AuthenticationBuisenessService : IAuthenticationBuisenessService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserSecurityService _userSecurityService;


        public AuthenticationBuisenessService(IUserRepository userRepository, IUserSecurityService userSecurityService)
        {
            if (userRepository == null)
                throw new ArgumentNullException("userRepository");
            if (userSecurityService == null)
                throw new ArgumentNullException("userSecurityService");

            _userRepository = userRepository;
            _userSecurityService = userSecurityService;
        }
        

        public AuthenticationData AuthenticateUser(string userName, string password)
        {
            if (String.IsNullOrEmpty(userName) || String.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException();
            }
            
            var user = _userRepository.GetUser(userName, UserType.User);
            if (user == null)
            {
                return new AuthenticationData()
                {
                    IsAuthenticated = false,
                    UserName = userName,
                    ErrorMessage = "We can't find you in our database."
                };
            }

            var isValidPassword = _userSecurityService.CheckPassword(user.PasswordHash, user.Salt, password);
            if (!isValidPassword)
            {
                return new AuthenticationData()
                {
                    IsAuthenticated = false,
                    UserName = userName,
                    ErrorMessage = "Wrong password."
                };
            }

            return new AuthenticationData()
            {
                UserName = user.UserName,
                IsAuthenticated = true,
                LogInTime = DateTime.Now
            };
        }

        public AuthenticationData AuthenticateService(string serviceName, string password)
        {
            if (String.IsNullOrEmpty(serviceName) || String.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException();
            }

            var service = _userRepository.GetUser(serviceName, UserType.Service);
            if (service == null)
            {
                return new AuthenticationData()
                {
                    IsAuthenticated = false,
                    UserName = serviceName,
                    ErrorMessage = "We can't find you in our database."
                };
            }

            var isValidPassword = _userSecurityService.CheckPassword(service.PasswordHash, service.Salt, password);
            if (!isValidPassword)
            {
                return new AuthenticationData()
                {
                    IsAuthenticated = false,
                    UserName = serviceName,
                    ErrorMessage = "Wrong password."
                };
            }

            return new AuthenticationData()
            {
                UserName = service.UserName,
                IsAuthenticated = true,
                LogInTime = DateTime.Now
            };
        }
    }
}
