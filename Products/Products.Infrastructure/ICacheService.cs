namespace Products.Infrastructure
{
    using System;

    public interface ICacheService
    {
        TValue Get<TValue>(string cacheKey, Func<TValue> getItemCallback) where TValue : class;
    }
}
