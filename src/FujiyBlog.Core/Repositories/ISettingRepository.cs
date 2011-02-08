using FujiyBlog.Core.DomainObjects;

namespace FujiyBlog.Core.Repositories
{
    public interface ISettingRepository
    {
        Setting MinRequiredPasswordLength { get; }
    }
}