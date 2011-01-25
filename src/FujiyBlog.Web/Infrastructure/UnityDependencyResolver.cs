using System;
using System.Collections.Generic;
using System.Web.Mvc;
using FujiyBlog.Core.Infrastructure;
using FujiyBlog.Core.Repositories;
using FujiyBlog.EntityFramework;
using Microsoft.Practices.Unity;
using System.Web;

namespace FujiyBlog.Web.Infrastructure
{
    public class UnityDependencyResolver : IDependencyResolver, IDisposable
    {
        private static IUnityContainer Container
        {
            get
            {
                IUnityContainer container = HttpContext.Current.Items["Container"] as IUnityContainer;
                if (container == null)
                {
                    container = new UnityContainer();
                    container.RegisterType<FujiyBlogDatabase, FujiyBlogDatabase>(new ContainerControlledLifetimeManager());
                    container.RegisterType<IUnitOfWork, FujiyBlogDatabase>(new ContainerControlledLifetimeManager());
                    container.RegisterType<IUserRepository, UserRepository>(new ContainerControlledLifetimeManager());
                    container.RegisterType<IPostRepository, PostRepository>(new ContainerControlledLifetimeManager());
                    container.RegisterType<IPostCommentRepository, PostCommentRepository>(new ContainerControlledLifetimeManager());
                    HttpContext.Current.Items["Container"] = container;
                }
                return container;
            }
        }

        public object GetService(Type serviceType)
        {
            if ((serviceType.IsClass && !serviceType.IsAbstract) || Container.IsRegistered(serviceType))
                return Container.Resolve(serviceType);
            return null;
        }


        public IEnumerable<object> GetServices(Type serviceType)
        {
            if ((serviceType.IsClass && !serviceType.IsAbstract) || Container.IsRegistered(serviceType))
                return Container.ResolveAll(serviceType);

            return new object[] {};
        }

        public void Dispose()
        {
            Container.Dispose();
        }
    }
}