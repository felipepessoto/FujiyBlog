using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.Repositories;
using FujiyBlog.Core.Resources;

namespace FujiyBlog.Core.Services
{
    public class UserService
    {
        private readonly IUserRepository userRepository;

        public UserService(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public CreateUserResult CreateUser(string username, string password, string email)
        {
            if(userRepository.EmailExistsWithAnotherUser(email, 0))
            {
                List<ValidationResult> validationResults = new List<ValidationResult>();
                validationResults.Add(new ValidationResult(TextMessages.UserEmailAlreadyExists, new[] {"Email"}));
                return new CreateUserResult(validationResults);
            }

            User user = new User();
            user.Username = username;
            user.Email = email;
            user.Password = password;

            userRepository.Add(user);

            return new CreateUserResult(user);
        }

        public bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            User user = userRepository.GetByUsername(username);

            if (user.Password == oldPassword)
            {
                user.Password = newPassword;
                return true;
            }

            return false;
        }

        public bool ValidateUser(string username, string password)
        {
            User user = userRepository.GetByUsername(username);
            return user != null && user.Password == password;
        }

        public void RetrievePassword(User user)
        {
            //TODO recuperar senha
        }
    }
}
