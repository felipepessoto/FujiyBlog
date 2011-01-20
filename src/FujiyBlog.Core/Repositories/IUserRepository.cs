using FujiyBlog.Core.DomainObjects;

namespace FujiyBlog.Core.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        bool EmailExistsWithAnotherUser(string email, int userId);
    }
}
