using CertiPay.Common.Logging;
using NUnit.Framework;

namespace CertiPay.Common.Tests.Logging
{
    public class LogManagerTests
    {
        [Test]
        public void Ensure_Can_Write_To_Rolling_File()
        {
            LogManager.GetCurrentClassLogger().Warn("This is some basic text output!");
        }

        [Test]
        public void Ensure_Can_Write_To_Email_Sinks()
        {
            LogManager.GetCurrentClassLogger().Fatal("An error occurred while running this test!");
        }

        [Test]
        public void Can_Include_Context()
        {
            LogManager
                .GetCurrentClassLogger()
                .WithContext("Test", true)
                .WithContext("OtherContext", new { id = 1, test = false }, destructureObjects: true)
                .Fatal("An error occured with context!");

            LogManager
                .GetCurrentClassLogger()
                .Fatal("An error occured without context!");
        }

        [Test]
        public void Include_Ambient_Context()
        {
            ILog log = LogManager.GetCurrentClassLogger();

            using (log.WithAmbientContext("property1", "pickles"))
            {
                log.Info("Should include property 1");

                using (log.WithAmbientContext("property2", "mustard"))
                {
                    log.Info("Should include property 1 and property 2");
                }

                log.Info("Should only include property 1");
            }

            log.Info("Should include neither property 1 nor property 2");
        }
    }
}