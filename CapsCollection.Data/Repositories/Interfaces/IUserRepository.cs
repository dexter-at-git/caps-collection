using CapsCollection.Data.Models;

namespace CapsCollection.Data.Repositories.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        User GetUser(string userName, UserType userType);
    }
}