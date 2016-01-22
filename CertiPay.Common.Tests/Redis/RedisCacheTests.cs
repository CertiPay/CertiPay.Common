using CertiPay.Common.Cache;
using CertiPay.Common.Logging;
using CertiPay.Common.Redis;
using NUnit.Framework;
using System;
using System.Linq;

namespace CertiPay.Services.Redis
{
    public class RedisCacheTests
    {
        private static readonly ILog Log = LogManager.GetLogger<RedisCacheTests>();

        [Test]
        public void Run_Redis_Simple_Load_Tests()
        {
            const int iterations = 1000;

            ICache cache = GetCache();

            using (Log.Timer("Loading Notifications from Redis"))
            {
                foreach (var iteration in Enumerable.Range(1, iterations))
                {
                    var notification = cache.GetOrAddAsync<RedisQueueManagerTests.TestMessage>("Notification:" + iteration, () =>
                    {
                        return new RedisQueueManagerTests.TestMessage
                        {
                            ID = Guid.NewGuid(),
                            Message = "Test message " + iteration
                        };
                    });
                }
            }
        }

        private ICache GetCache()
        {
            RedisConnection conn = new RedisConnection
            {
                // TODO -- Uses local default connection
            };

            return new RedisCache(conn);
        }
    }
}