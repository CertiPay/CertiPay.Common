namespace CertiPay.Common.Logging
{
    using System;
using System.Collections.Generic;

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
        /// Log a templated message at the given log level with properties
        /// </summary>
        /// <param name="level">An enumeration representing the different levels of attention for logging</param>
        /// <param name="properties">An key value dictionary containing property name value pair not included in messageTemplate</param>
        /// <param name="messageTemplate">A string message that accepted templated values (either {0} {1} or {prop_1} {prop_2}) a la String.Format</param>
        /// <param name="propertyValues">The properties to replace in the message template</param>
        void Log(LogLevel level, Dictionary<string, object> properties, string messageTemplate, params object[] propertyValues);

        /// <summary>
        /// Log a templated message at the given log level with properties
        /// </summary>
        /// <typeparam name="TException">The exception that occurred to log the stack trace for</typeparam>
        /// <param name="level">An enumeration representing the different levels of attention for logging</param>
        /// <param name="properties">An key value dictionary containing property name value pair not included in messageTemplate</param>
        /// <param name="messageTemplate">A string message that accepted templated values (either {0} {1} or {prop_1} {prop_2}) a la String.Format</param>
        /// <param name="propertyValues">The properties to replace in the message template</param>
        void Log<TException>(LogLevel level, Dictionary<string, object> properties, string messageTemplate, TException exception, params object[] propertyValues) where TException : Exception;
    }
}