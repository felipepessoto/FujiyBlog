using System.Linq;
using FujiyBlog.Core.DomainObjects;

namespace FujiyBlog.Core.EntityFramework
{
    public class UserRepository : RepositoryBase<User>
    {
        public UserRepository(FujiyBlogDatabase database)
            : base(database)
        {
        }

        public bool EmailExistsWithAnotherUser(string email, int userId)
        {
            return Database.Users.Any(x => x.Email == email && x.Id != userId);
        }

        public User GetById(int id)
        {
            return Database.Users.SingleOrDefault(x => x.Id == id);
        }

        public User GetByUsername(string username)
        {
            return Database.Users.SingleOrDefault(x => x.Username == username);
        }

        public User GetByEmail(string email)
        {
            return Database.Users.SingleOrDefault(x => x.Email == email);
        }
    }
}