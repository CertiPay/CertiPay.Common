using CertiPay.Common.Cache;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace CertiPay.Common.Redis
{
    /// <summary>
    /// An instance of the ICache that stores cache data in a redis instance
    /// </summary>
    public class RedisCache : ICache
    {
        private readonly RedisConnection _connection;

        public RedisCache(RedisConnection connection)
        {
            this._connection = connection;
        }

        public TimeSpan DefaultExpiration { get { return TimeSpan.FromDays(1); } }

        public T GetOrAdd<T>(string key, Func<T> factory)
        {
            return GetOrAdd<T>(key, factory, DefaultExpiration);
        }

        public T GetOrAdd<T>(string key, Func<T> factory, TimeSpan expiration)
        {
            T value = default(T);

            if (!TryGet(key, out value))
            {
                value = factory.Invoke();

                Add(key, value);
            }

            return value;
        }

        public Task<T> GetOrAddAsync<T>(string key, Func<T> factory)
        {
            return GetOrAddAsync<T>(key, factory, DefaultExpiration);
        }

        public async Task<T> GetOrAddAsync<T>(string key, Func<T> factory, TimeSpan expiration)
        {
            T value = default(T);

            if (!TryGet(key, out value))
            {
                value = factory.Invoke();

                await AddAsync(key, value);
            }

            return value;
        }

        public Boolean TryGet<T>(string key, out T value)
        {
            value = default(T);

            string json = _connection.GetClient().StringGet(key);

            if (String.IsNullOrWhiteSpace(json)) return false;

            value = JsonConvert.DeserializeObject<T>(json);

            return true;
        }

        public void Add<T>(string key, T value)
        {
            _connection.GetClient().StringSet(key, JsonConvert.SerializeObject(value));
        }

        public async Task AddAsync<T>(string key, T value)
        {
            await _connection.GetClient().StringSetAsync(key, JsonConvert.SerializeObject(value)).ConfigureAwait(false);
        }

        public async Task Remove(string key)
        {
            await _connection.GetClient().KeyDeleteAsync(key, CommandFlags.FireAndForget).ConfigureAwait(false);
        }

        public async Task Flush()
        {
            await _connection.GetServer().FlushDatabaseAsync(_connection.DefaultDb);
        }
    }
}