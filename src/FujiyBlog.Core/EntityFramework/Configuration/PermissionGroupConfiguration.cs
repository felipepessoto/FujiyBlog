using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq.Expressions;
using System.Reflection;
using FujiyBlog.Core.DomainObjects;

namespace FujiyBlog.Core.EntityFramework.Configuration
{
    public class PermissionGroupConfiguration : EntityTypeConfiguration<PermissionGroup>
    {
        public PermissionGroupConfiguration()
        {
            Property(b => b.Name).IsUnicode(false);
            PropertyStr(this, "AssignedPermissions").IsUnicode(false);
            Ignore(x => x.Permissions);
        }

        private static StringPropertyConfiguration PropertyStr<T>(EntityTypeConfiguration<T> mapper, String propertyName)
            where T : class
        {
            Type type = typeof (T);
            ParameterExpression arg = Expression.Parameter(type, "x");
            Expression expr = arg;

            PropertyInfo pi = type.GetProperty(propertyName,
                                               BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            expr = Expression.Property(expr, pi);

            LambdaExpression lambda = Expression.Lambda(expr, arg);

            Expression<Func<T, String>> expression = (Expression<Func<T, string>>) lambda;
            return mapper.Property(expression);
        }

        private static ManyNavigationPropertyConfiguration<T, U> HasMany<T, U>(EntityTypeConfiguration<T> mapper, String propertyName)
            where T : class
            where U : class
        {
            Type type = typeof (T);
            ParameterExpression arg = Expression.Parameter(type, "x");
            Expression expr = arg;

            PropertyInfo pi = type.GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            expr = Expression.Property(expr, pi);

            LambdaExpression lambda = Expression.Lambda(expr, arg);

            Expression<Func<T, ICollection<U>>> expression = (Expression<Func<T, ICollection<U>>>) lambda;

            return mapper.HasMany(expression);

        }
    }
}
