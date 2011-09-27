using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace FujiyBlog.Core.DomainObjects
{
    public class PermissionGroup
    {
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string Name { get; set; }

        public IEnumerable<Permission> Permissions
        {
            get
            {
                return (AssignedPermissions ?? string.Empty).Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(x => (Permission)Enum.Parse(typeof(Permission), x));
            }
            set
            {
                AssignedPermissions = string.Join(",", value.Select(x => x.ToString()));
            }
        }

        public ICollection<User> Users { get; set; }

        private string AssignedPermissions { get; set; }
    }
}
