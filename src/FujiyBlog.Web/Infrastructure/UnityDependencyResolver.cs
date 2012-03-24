using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using FujiyBlog.Core.EntityFramework;
using Microsoft.Practices.Unity;

namespace FujiyBlog.Web.Infrastructure
{
    public class UnityDependencyResolver : IDependencyResolver, IDisposable
    {
        private static IUnityContainer Container
        {
            get
            {
                IUnityContainer container = null;
                if (HttpContext.Current != null)
                {
                    container = HttpContext.Current.Items["Container"] as IUnityContainer;
                }
                if (container == null)
                {
                    container = new UnityContainer();
                    container.RegisterType<FujiyBlogDatabase, FujiyBlogDatabase>(new ContainerControlledLifetimeManager());
                    container.RegisterType<UserRepository, UserRepository>(new ContainerControlledLifetimeManager());
                    container.RegisterType<SettingRepository, SettingRepository>(new ContainerControlledLifetimeManager());
                    container.RegisterType<FeedRepository, FeedRepository>(new ContainerControlledLifetimeManager());
                    container.RegisterType<WidgetSettingRepository, WidgetSettingRepository>(new ContainerControlledLifetimeManager());

                    if (HttpContext.Current != null)
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