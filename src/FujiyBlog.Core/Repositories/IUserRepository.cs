using FujiyBlog.Core.DomainObjects;

namespace FujiyBlog.Core.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        bool EmailExistsWithAnotherUser(string email, int userId);
        User GetById(int id);
        User GetByUsername(string username);
        User GetByEmail(string email);
    }
}
