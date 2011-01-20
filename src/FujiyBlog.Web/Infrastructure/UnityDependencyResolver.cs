using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.Practices.Unity;

namespace FujiyBlog.Web.Infrastructure
{
    public class UnityDependencyResolver : IDependencyResolver
    {
        private readonly IUnityContainer container;

        public UnityDependencyResolver(IUnityContainer container)
        {
            this.container = container;
        }

        public object GetService(Type serviceType)
        {
            if ((serviceType.IsClass && !serviceType.IsAbstract) || container.IsRegistered(serviceType))
                return container.Resolve(serviceType);
            return null;
        }


        public IEnumerable<object> GetServices(Type serviceType)
        {
            if ((serviceType.IsClass && !serviceType.IsAbstract) || container.IsRegistered(serviceType))
                return container.ResolveAll(serviceType);

            return new object[] {};
        }
    }
}