using CertiPay.Common.Cache;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System;

namespace CertiPay.Common.Testing
{
    /// <summary>
    /// Flush the InMemoryCache instance before and after the test execution
    /// </summary>
    public class CacheFlushAttribute : Attribute, ITestAction
    {
        public ActionTargets Targets { get { return ActionTargets.Test; } }

        private static readonly ICache cache = new InMemoryCache();

        public void AfterTest(ITest test)
        {
            cache.Flush().Wait();
        }

        public void BeforeTest(ITest test)
        {
            cache.Flush().Wait();
        }
    }
}