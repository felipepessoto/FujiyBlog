using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FujiyBlog.Core.DomainObjects;

namespace FujiyBlog.Core.Services
{
    public class CreateUserResult : ServiceResultBase
    {
        public User User
        {
            get;
            private set;
        }

        public CreateUserResult()
            : this(new ValidationResult[0])
        {
        }

        public CreateUserResult(User user)
            : this()
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            User = user;
        }

        public CreateUserResult(IEnumerable<ValidationResult> validationResults)
            : base(validationResults)
        {
        }
    }
}
