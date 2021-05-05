namespace Products.Infrastructure
{
    using System;
    using System.Runtime.Caching;

    public class CacheManager : ICacheService
    {
        public TValue Get<TValue>(string cacheKey, Func<TValue> getItemCallback) where TValue : class
        {
            // ReSharper disable once UseNegatedPatternMatching
            var item = MemoryCache.Default.Get(cacheKey) as TValue;
            if (item == null)
            {
                item = getItemCallback();
                MemoryCache.Default.Add(cacheKey, item, DateTime.Now.AddDays(1));
            }
            return item;
        }
    }
}
