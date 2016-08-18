namespace CapsCollection.Data.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string Salt { get; set; }
        public UserType UserType { get; set; }
        public bool IsDisabled { get; set; }
    }
}