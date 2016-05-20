using CertiPay.Common.Logging;
using NUnit.Framework;
using System;
using System.Threading;

namespace CertiPay.Common.Tests.Logging
{
    public class MetricLoggingExtensionsTests
    {
        private static readonly ILog Log = LogManager.GetLogger<MetricLoggingExtensionsTests>();

        [Test]
        public void Use_Log_Timer_No_Identifier()
        {
            using (Log.Timer("Use_Log_Timer"))
            {
                // Cool stuff happens here
            }
        }

        [Test]
        public void Takes_Longer_Than_Threshold()
        {
            using (Log.Timer("Takes_Longer_Than_Threshold", warnIfExceeds: TimeSpan.FromMilliseconds(100)))
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(150));
            }
        }

        [Test]
        public void Object_Context_Provided()
        {
            using (Log.Timer("Object_Context_Provided {id} {userId}", new { id = 10, userId = 12 }))
            {
                // Cool stuff happens here
            }
        }
    }
}