using System.Linq;
using CapsCollection.Data.DataContext;
using CapsCollection.Data.Models;
using CapsCollection.Data.Repositories.Interfaces;

namespace CapsCollection.Data.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly CapsCollectionContext _context;

        public UserRepository(CapsCollectionContext context) : base(context)
        {
            _context = context;
        }

        public User GetUser(string userName, UserType userType)
        {
            var user = _context.Users.FirstOrDefault(x => x.UserName == userName && x.UserType == userType && !x.IsDisabled);

            return user;
        }
    }
}
