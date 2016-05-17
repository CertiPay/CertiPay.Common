namespace CertiPay.Common.Testing
{
    using NUnit.Framework;
    using NUnit.Framework.Interfaces;
    using System;
    using System.Transactions;

    /// <summary>
    /// Allows a test to roll the database back to at the end to return the system to the previous state
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
    public class AutoRollbackAttribute : Attribute, ITestAction, IDisposable
    {
        public ActionTargets Targets { get { return ActionTargets.Test; } }

        private TransactionScope _scope;

        public AutoRollbackAttribute()
        {
            IsolationLevel = IsolationLevel.Unspecified;
            ScopeOption = TransactionScopeOption.Required;
        }

        public IsolationLevel IsolationLevel { get; set; }

        public TransactionScopeOption ScopeOption { get; set; }

        public int TimeOutInSeconds { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "_scope")]
        public void Dispose()
        {
            // Nothing to do here. TransactionScope is disposed after test completion
        }

        public void BeforeTest(ITest test)
        {
            var options = new TransactionOptions { IsolationLevel = this.IsolationLevel };

            if (TimeOutInSeconds > 0)
            {
                options.Timeout = TimeSpan.FromSeconds(this.TimeOutInSeconds);
            }

            this._scope = new TransactionScope(this.ScopeOption, options);
        }

        public void AfterTest(ITest test)
        {
            this._scope.Dispose();
        }
    }
}