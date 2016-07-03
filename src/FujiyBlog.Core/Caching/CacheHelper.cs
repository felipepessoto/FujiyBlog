using FujiyBlog.Core.EntityFramework;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;

namespace FujiyBlog.Core.Caching
{
    public static class CacheHelper
    {
        private static string lastDatabaseChange;
        private static IMemoryCache DefaultCache = new MemoryCache(new MemoryCacheOptions());

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "MethodCallExpression"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Não há outra técnica para isto. E não aumenta a complexidade já que a expression é um syntactic sugar, o cliente apenas escreve um lambda")]
        public static TResult FromCacheOrExecute<TResult>(FujiyBlogDatabase db, Expression<Func<TResult>> func, string key = null, MemoryCacheEntryOptions cacheItemPolicy = null, bool condition = true)
        {
            string lastDatabaseChangeAtDb = db.LastDatabaseChange;

            if (lastDatabaseChangeAtDb != lastDatabaseChange)
            {
                lastDatabaseChange = lastDatabaseChangeAtDb;
                ClearCache();
            }

            if (!condition)
            {
                return func.Compile()();
            }

            if (func == null)
                throw new ArgumentNullException("func");

            if (string.IsNullOrEmpty(key))
            {
                MethodCallExpression method = func.Body as MethodCallExpression;

                if (method == null)
                {
                    throw new InvalidCachedFuncException("Body must be MethodCallExpression to auto generate a cache key");
                }

                key = CacheKeyGenerator.GenerateKey(method);
            }

            object returnObject = DefaultCache.Get(key);

            if (!(returnObject is TResult))
            {
                returnObject = func.Compile()();
                if (returnObject != null)
                {
                    DefaultCache.Set(key, returnObject, cacheItemPolicy);
                }
            }

            return (TResult) returnObject;
        }

        public static void ClearCache()
        {
            var oldCache = DefaultCache;
            DefaultCache = new MemoryCache(new MemoryCacheOptions());
            oldCache.Dispose();
        }
    }
}
