namespace CertiPay.Common.Logging
{
    using System;

    /// <summary>
    /// A generic interface for writing log entries to various destinations. The intent behind this interface
    /// is to avoid taking a direct dependency on a logger implementation or abstraction (i.e. commons.logging).
    /// </summary>
    /// <remarks>
    /// This interface is inspired after LibLog by DamianH and RavenDB's logging abstractions
    /// </remarks>
    public interface ILog
    {
        /// <summary>
        /// Log a templated message at the given log level with properties
        /// </summary>
        /// <param name="level">An enumeration representing the different levels of attention for logging</param>
        /// <param name="messageTemplate">A string message that accepted templated values (either {0} {1} or {prop_1} {prop_2}) a la String.Format</param>
        /// <param name="propertyValues">The properties to replace in the message template</param>
        void Log(LogLevel level, string messageTemplate, params object[] propertyValues);

        /// <summary>
        /// Log a templated message at the given log level with properties
        /// </summary>
        /// <typeparam name="TException">The exception that occurred to log the stack trace for</typeparam>
        /// <param name="level">An enumeration representing the different levels of attention for logging</param>
        /// <param name="messageTemplate">A string message that accepted templated values (either {0} {1} or {prop_1} {prop_2}) a la String.Format</param>
        /// <param name="propertyValues">The properties to replace in the message template</param>
        void Log<TException>(LogLevel level, string messageTemplate, TException exception, params object[] propertyValues) where TException : Exception;

        /// <summary>
        /// Provide additional context for the log entry that might not necessarily be represented in the message output
        /// </summary>
        /// <param name="propertyName">The name to store the context with</param>
        /// <param name="value">The value to store</param>
        /// <param name="destructureObjects">If true, will destructure (serialize) the object for storage. Defaults to false.</param>
        ILog WithContext(String propertyName, Object value, Boolean destructureObjects = false);

        /// <summary>
        /// Push a property onto the context, returning an <see cref="IDisposable"/>
        /// that can later be used to remove the property, along with any others that
        /// may have been pushed on top of it and not yet popped. The property must
        /// be popped from the same thread/logical call context.
        /// </summary>
        /// <param name="propertyName">The name to store the context with</param>
        /// <param name="value">The value to store</param>
        /// <param name="destructureObjects">If true, will destructure (serialize) the object for storage. Defaults to false.</param>
        IDisposable WithAmbientContext(String propertyName, Object value, Boolean destructureObjects = false);
    }
}