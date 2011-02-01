using System;
using System.Web.Security;
using FujiyBlog.Core.DomainObjects;

namespace FujiyBlog.Web.Infrastructure
{
    public class BlogMembershipUser : MembershipUser
    {
        private readonly User user;

        public BlogMembershipUser(string providerName, User user)
            : base(providerName, user.Username, null, user.Email, null, null, true, false,
                user.CreationDate, user.LastLoginDate, DateTime.MinValue, DateTime.MinValue,DateTime.MinValue)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            this.user = user;
        }

        public int Id { get { return user.Id; }}

        public override object ProviderUserKey { get { return user.Id; } }

        public string DisplayName { get { return user.DisplayName; } }

        public string FullName { get { return user.FullName; } }

        public string Location { get { return user.Location; } }

        public string About { get { return user.About; } }

        public DateTime? BirthDate { get { return user.BirthDate; } }
    }
}