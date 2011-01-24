namespace FujiyBlog.Core.Infrastructure
{
    public interface IUnitOfWork
    {
        void SaveChanges();
    }
}
