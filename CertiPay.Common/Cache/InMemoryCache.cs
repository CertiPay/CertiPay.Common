using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace CertiPay.Common.Cache
{
    /// <summary>
    /// Basic implementation of a cache that stores items in memory
    /// utilizing the System.Runtime.Caching.MemoryCache instance
    /// </summary>
    public class InMemoryCache : ICache
    {
        /// <summary>
        /// Maintains an in-memory list of the keys used in the cache for a more efficient mechanism for clearing it.
        /// Dispose() would blow up any future usage, and enumerating the cache is memory intensive since it also involves the objects
        /// </summary>
        private static readonly IDictionary<String, String> keys = new ConcurrentDictionary<String, String>();

        public TimeSpan DefaultExpiration { get; set; }

        public InMemoryCache() : this(TimeSpan.FromDays(1))
        {
            // Nothing to do here
        }

        public InMemoryCache(TimeSpan defaultExpiration)
        {
            this.DefaultExpiration = defaultExpiration;
        }

        public T GetOrAdd<T>(string key, Func<T> factory)
        {
            return GetOrAdd<T>(key, factory, DefaultExpiration);
        }

        public T GetOrAdd<T>(string key, Func<T> factory, TimeSpan expiration)
        {
            T val = default(T);

            if (!TryGet(key, out val))
            {
                val = factory.Invoke();

                Add(key, val, expiration);
            }

            return val;
        }

        public Task<T> GetOrAddAsync<T>(string key, Func<T> factory)
        {
            return GetOrAddAsync(key, factory, DefaultExpiration);
        }

        public Task Remove(string key)
        {
            MemoryCache.Default.Remove(key);
            keys.Remove(key);
            return Task.FromResult(0);
        }

        public Task<T> GetOrAddAsync<T>(string key, Func<T> factory, TimeSpan expiration)
        {
            return Task.FromResult(GetOrAdd(key, factory, expiration));
        }

        public void Add<T>(String key, T val, TimeSpan expiration)
        {
            MemoryCache.Default.Add(key, val, DateTime.Now.Add(expiration));
            keys.Add(key, String.Empty);
        }

        public Boolean TryGet<T>(String key, out T val)
        {
            val = default(T);

            if (!MemoryCache.Default.Contains(key))
            {
                return false;
            }

            val = (T)MemoryCache.Default.Get(key);

            return true;
        }

        public Task Flush()
        {
            foreach (var key in keys)
            {
                Remove(key.Key);
            }

            return Task.FromResult(0);
        }
    }
}