using NUnit.Framework;

namespace CertiPay.Common.Tests
{
    public class UtilitiesTests
    {
        [Test, Ignore("This doesn't work on AppVeyor since we patch the properties")]
        public void ShouldMatchVersionOfCaller()
        {
            Assert.AreEqual("0.9.9.local", Utilities.Version<UtilitiesTests>());
        }
    }
}