using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using FujiyBlog.Core.Caching;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.EntityFramework;

namespace FujiyBlog.Web.Infrastructure
{
    public class FujiyBlogRoleProvider : RoleProvider
    {
        public override bool IsUserInRole(string username, string roleName)
        {
            throw new NotImplementedException();
        }

        public override string[] GetRolesForUser(string username)
        {
            return CacheHelper.FromCacheOrExecute(() => InternalGetRolesForUser(username));
        }

        private static string[] InternalGetRolesForUser(string username)
        {
            FujiyBlogDatabase db = DependencyResolver.Current.GetService<FujiyBlogDatabase>();

            List<RoleGroup> roleGroups = db.RoleGroups.AsNoTracking().Where(x => x.Users.Any(y => y.Username == username && y.Enabled)).ToList();

            return roleGroups.SelectMany(x => x.Roles).Select(x => x.ToString()).ToArray();
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName
        {
            get { return "FujiyBlog"; }
            set { throw new NotImplementedException(); }
        }
    }
}