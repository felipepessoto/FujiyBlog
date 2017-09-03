using FujiyBlog.Core.EntityFramework;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace FujiyBlog.Core.Caching
{
    public static class CacheHelper
    {
        private static string lastDatabaseChange;
        private static IMemoryCache DefaultCache = new MemoryCache(new MemoryCacheOptions());

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "MethodCallExpression"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Não há outra técnica para isto. E não aumenta a complexidade já que a expression é um syntactic sugar, o cliente apenas escreve um lambda")]
        public static TResult FromCacheOrExecute<TResult>(FujiyBlogDatabase db, Expression<Func<TResult>> func, MemoryCacheEntryOptions cacheItemPolicy = null, bool condition = true)
        {
            if (func == null)
            {
                throw new ArgumentNullException("func");
            }

            string key = ExtractKeyFromExpression(func);

            return FromCacheOrExecute(db, func.Compile(), key, cacheItemPolicy, condition);
        }

        public static TResult FromCacheOrExecute<TResult>(FujiyBlogDatabase db, Func<TResult> func, string key, MemoryCacheEntryOptions cacheItemPolicy = null, bool condition = true)
        {
            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }

            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            string lastDatabaseChangeAtDb = db.LastDatabaseChange;

            if (lastDatabaseChangeAtDb != lastDatabaseChange)
            {
                lastDatabaseChange = lastDatabaseChangeAtDb;
                ClearCache();
            }

            if (!condition)
            {
                return func();
            }

            object returnObject = DefaultCache.Get(key);

            bool hasValueAndIsSameType = (returnObject is TResult);

            if (hasValueAndIsSameType)
            {
                Task taskValue = returnObject as Task;
                if (taskValue != null && (taskValue.Status == TaskStatus.Canceled || taskValue.Status == TaskStatus.Faulted))
                {
                    hasValueAndIsSameType = false;
                }
            }

            if (hasValueAndIsSameType == false)
            {
                returnObject = func();
                if (returnObject != null)
                {
                    DefaultCache.Set(key, returnObject, cacheItemPolicy);
                }
            }

            return (TResult)returnObject;
        }

        private static string ExtractKeyFromExpression<TResult>(Expression<Func<TResult>> func)
        {
            string key;
            MethodCallExpression method = func.Body as MethodCallExpression;

            if (method == null)
            {
                throw new InvalidCachedFuncException("Body must be MethodCallExpression to auto generate a cache key");
            }

            key = CacheKeyGenerator.GenerateKey(method);
            return key;
        }

        public static void ClearCache()
        {
            var oldCache = DefaultCache;
            DefaultCache = new MemoryCache(new MemoryCacheOptions());
            oldCache.Dispose();
        }
    }
}
