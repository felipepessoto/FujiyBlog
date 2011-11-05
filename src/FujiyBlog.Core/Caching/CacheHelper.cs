using System;
using System.Linq.Expressions;
using System.Runtime.Caching;

namespace FujiyBlog.Core.Caching
{
    public static class CacheHelper
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "MethodCallExpression"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Não há outra técnica para isto. E não aumenta a complexidade já que a expression é um syntactic sugar, o cliente apenas escreve um lambda")]
        public static TResult FromCacheOrExecute<TResult>(Expression<Func<TResult>> func, string key = null, CacheItemPolicy cacheItemPolicy = null, bool condition = true)
        {
            if (!condition)
            {
                return func.Compile()();
            }

            MemoryCache cache = MemoryCache.Default;

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

            object returnObject = cache[key];

            if (!(returnObject is TResult))
            {
                returnObject = func.Compile()();
                if (returnObject != null)
                {
                    cache.Set(key, returnObject, cacheItemPolicy);
                }
            }

            return (TResult) returnObject;
        }
    }
}
