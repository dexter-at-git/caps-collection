using System;

namespace CapsCollection.Common.Models
{
    public class AuthenticationData
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsAuthenticated { get; set; }
        public DateTime LogInTime { get; set; }
        public Type UserType { get; set; }
        public string ErrorMessage { get; set; }
    }

    public enum Type
    {
        ServiceUser,
        User
    }
}
