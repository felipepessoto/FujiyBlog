using System;
using System.Linq;
using System.Web.Security;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.Repositories;
using FujiyBlog.Core.Services;
using System.Web.Mvc;

namespace FujiyBlog.Web.Infrastructure
{
    public class BlogMembershipProvider : MembershipProvider
    {
        private static UserService UserService
        {
            get
            {
                return (UserService)DependencyResolver.Current.GetService(typeof(UserService));
            }
        }

        private static IUserRepository UserRepository
        {
            get
            {
                return (IUserRepository)DependencyResolver.Current.GetService(typeof(IUserRepository));
            }
        }

        private int minRequiredPasswordLength;

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            base.Initialize(name, config);

            minRequiredPasswordLength = int.Parse(config["minRequiredPasswordLength"]);
        }

        #region Overrides of MembershipProvider

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            CreateUserResult createUserResult = UserService.CreateUser(username, password, email);

            if (createUserResult.RuleViolations.Any(x => x.MemberNames.First() == "Email"))
            {
                status = MembershipCreateStatus.DuplicateEmail;
                return null;
            }

            if (!createUserResult.RuleViolations.Any())
            {
                User newUser = createUserResult.User;
                status = MembershipCreateStatus.Success;
                return new BlogMembershipUser(Name, newUser);
            }

            status = MembershipCreateStatus.ProviderError;
            return null;
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            User user = UserRepository.GetByUsername(username);

            if (user.Password == oldPassword)
            {
                user.Password = newPassword;
                return true;
            }

            return false;
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override void UpdateUser(MembershipUser user)
        {
            BlogMembershipUser membershipUser = (BlogMembershipUser)user;

            User blogUser = UserRepository.GetById(membershipUser.Id);

            if (blogUser.About != membershipUser.About)
            {
                blogUser.About = membershipUser.About;
            }

            if (blogUser.BirthDate != membershipUser.BirthDate)
            {
                blogUser.BirthDate = membershipUser.BirthDate;
            }

            if (blogUser.DisplayName != membershipUser.DisplayName)
            {
                blogUser.DisplayName = membershipUser.DisplayName;
            }

            if (blogUser.Email != membershipUser.Email)
            {
                blogUser.Email = membershipUser.Email;
            }

            if (blogUser.FullName != membershipUser.FullName)
            {
                blogUser.FullName = membershipUser.FullName;
            }

            if (blogUser.Location != membershipUser.Location)
            {
                blogUser.Location = membershipUser.Location;
            }
        }

        public override bool ValidateUser(string username, string password)
        {
            User user = UserRepository.GetByUsername(username);
            return user != null && user.Password == password;
        }

        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            User user = UserRepository.GetById((int)providerUserKey);

            if(user == null)
            {
                return null;
            }

            if (userIsOnline)
            {
                user.LastLoginDate = DateTime.Now;
            }

            return new BlogMembershipUser(Name, user);
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            User user = UserRepository.GetByUsername(username);

            if (user == null)
            {
                return null;
            }

            if (userIsOnline)
            {
                user.LastLoginDate = DateTime.Now;
            }

            return new BlogMembershipUser(Name, user);
        }

        public override string GetUserNameByEmail(string email)
        {
            User user = UserRepository.GetByEmail(email);
            if (user != null)
            {
                return user.Username;
            }
            return null;
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override bool EnablePasswordRetrieval
        {
            get { throw new NotImplementedException(); }
        }

        public override bool EnablePasswordReset
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { throw new NotImplementedException(); }
        }

        public override string ApplicationName
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { throw new NotImplementedException(); }
        }

        public override int PasswordAttemptWindow
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresUniqueEmail
        {
            get { return true; }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredPasswordLength
        {
            get { return minRequiredPasswordLength; }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { throw new NotImplementedException(); }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}