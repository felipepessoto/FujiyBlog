using System.Data.Entity.ModelConfiguration;
using FujiyBlog.Core.DomainObjects;

namespace FujiyBlog.Core.EntityFramework.Configuration
{
    public class RoleGroupConfiguration : EntityTypeConfiguration<RoleGroup>
    {
        public RoleGroupConfiguration()
        {
            Property(x=> x.AssignedRoles).IsMaxLength();
            Ignore(x => x.Roles);
        }
    }
}
