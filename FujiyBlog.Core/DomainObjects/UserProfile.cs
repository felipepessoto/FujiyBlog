using System;

namespace FujiyBlog.Core.DomainObjects
{
    public class UserProfile
    {
        public virtual int UserId { get; set; }//TODO Verificar melhor maneira

        public string About { get; set; }

        public DateTime BirthDate { get; set; }

        public string Location { get; set; }

        public string DisplayName { get; set; }

        public string FullName { get; set; }
    }
}
