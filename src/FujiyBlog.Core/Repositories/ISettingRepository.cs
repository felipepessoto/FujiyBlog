using System;

namespace FujiyBlog.Core.Repositories
{
    public interface ISettingRepository
    {
        int MinRequiredPasswordLength { get; }
        int PostsPerPage { get; }
        string BlogName { get; }
        string BlogDescription { get; }
        string Theme { get; }
        TimeZoneInfo TimeZoneId { get; }
    }
}