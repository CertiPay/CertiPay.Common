using System;
using System.Threading.Tasks;

namespace CertiPay.Common.Cache
{
    /// <summary>
    /// Provides a simple abstraction for caching items for a specified duration for quick access
    /// </summary>
    public interface ICache
    {
        /// <summary>
        /// The default amount of time an item is in the cache if none is specified during insertion
        /// </summary>
        TimeSpan DefaultExpiration { get; }

        /// <summary>
        /// Act as a read-through cache, trying to get the item from the cache
        /// if it exists, and adding it to the cache if it does not with the default
        /// expiration period
        /// </summary>
        T GetOrAdd<T>(String key, Func<T> factory);

        /// <summary>
        /// Act as a read-through cache, trying to get the item from the cache
        /// if it exists, and adding it to the cache if it does not with the default
        /// expiration period
        /// </summary>
        Task<T> GetOrAddAsync<T>(String key, Func<T> factory);

        /// <summary>
        /// Act as a read-through cache, trying to get the item from the cache
        /// if it exists, and adding it to the cache if it does not with the given
        /// expiration period
        /// </summary>
        T GetOrAdd<T>(String key, Func<T> factory, TimeSpan expiration);

        /// <summary>
        /// Act as a read-through cache, trying to get the item from the cache
        /// if it exists, and adding it to the cache if it does not with the given
        /// expiration period
        /// </summary>
        Task<T> GetOrAddAsync<T>(String key, Func<T> factory, TimeSpan expiration);

        /// <summary>
        /// Remove an item from the cache with the given key
        /// </summary>
        Task Remove(String key);
    }
}