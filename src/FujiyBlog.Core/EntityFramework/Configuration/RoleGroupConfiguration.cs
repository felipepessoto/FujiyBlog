using System.Data.Entity.ModelConfiguration;
using FujiyBlog.Core.DomainObjects;

namespace FujiyBlog.Core.EntityFramework.Configuration
{
    public class RoleGroupConfiguration : EntityTypeConfiguration<RoleGroup>
    {
        public RoleGroupConfiguration()
        {
            Property(b => b.Name).IsUnicode(false);
            Property(x=> x.AssignedRoles).IsUnicode(false).IsMaxLength();
            Ignore(x => x.Roles);
        }
    }
}
