using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace FujiyBlog.Core.DomainObjects
{
    public class RoleGroup
    {
        public RoleGroup()
        {
            Users = new List<User>();
        }

        public int Id { get; set; }

        [Required, StringLength(50)]
        public string Name { get; set; }

        public IEnumerable<Role> Roles
        {
            get
            {
                return (AssignedRoles ?? string.Empty).Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(x => (Role)Enum.Parse(typeof(Role), x));
            }
            set
            {
                AssignedRoles = string.Join(",", value.Select(x => x.ToString()));
            }
        }

        public ICollection<User> Users { get; set; }

        public string AssignedRoles { get; set; }
    }
}
