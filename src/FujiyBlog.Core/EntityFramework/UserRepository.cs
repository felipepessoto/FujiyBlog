using System.Linq;
using FujiyBlog.Core.DomainObjects;

namespace FujiyBlog.Core.EntityFramework
{
    public class UserRepository : RepositoryBase<ApplicationUser>
    {
        public UserRepository(FujiyBlogDatabase database)
            : base(database)
        {
        }

        public bool EmailExistsWithAnotherUser(string email, string userId)
        {
            return Database.Users.Any(x => x.Email == email && x.Id != userId);
        }

        public ApplicationUser GetById(string id)
        {
            return Database.Users.SingleOrDefault(x => x.Id == id);
        }

        public ApplicationUser GetByUsername(string username)
        {
            return Database.Users.SingleOrDefault(x => x.UserName == username);
        }

        public ApplicationUser GetByEmail(string email)
        {
            return Database.Users.SingleOrDefault(x => x.Email == email);
        }
    }
}