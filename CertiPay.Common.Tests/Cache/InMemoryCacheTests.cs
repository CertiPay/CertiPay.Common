using CertiPay.Common.Cache;
using NUnit.Framework;
using System.Threading.Tasks;

namespace CertiPay.Common.Tests.Cache
{
    public class InMemoryCacheTests
    {
        [Test]
        public void Confirm_Get_Or_Add()
        {
            ICache cache = new InMemoryCache();

            var result = cache.GetOrAdd("Test", () => "Testy Test");

            var result2 = cache.GetOrAdd("Test", () => "Not Testy Test");

            // They should be the same since the 'Test' key is still in the cache

            Assert.AreEqual(result, result2);
        }

        [Test]
        public async Task Confirm_Cache_Flush()
        {
            ICache cache = new InMemoryCache();

            var result = cache.GetOrAdd("Test", () => "Testy Test");

            await cache.Flush();

            var result2 = cache.GetOrAdd("Test", () => "Not Testy Test");

            // They should be different since the cache was flushed, and item gets re-added

            Assert.AreNotEqual(result, result2);
        }
    }
}