using System;

namespace FujiyBlog.Core.Repositories
{
    public interface ISettingRepository
    {
        int MinRequiredPasswordLength { get; set; }
        int PostsPerPage { get; set; }
        string BlogName { get; set; }
        string BlogDescription { get; set; }
        string Theme { get; set; }
        TimeZoneInfo TimeZoneId { get; set; }
    }
}