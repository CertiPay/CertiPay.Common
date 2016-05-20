namespace CertiPay.Common.Logging
{
    using System;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>
    /// Provides extensions for logging events for metric analysis. Metric names should be in a format that is easy to parse.
    /// </summary>
    /// <remarks>
    /// Inspired by StatsD and MiniProfiler, and most especially Serilog.Metrics extensions
    /// </remarks>
    public static class MetricLoggingExtensions
    {
        /// <summary>
        /// Track the execution time of a particular block of code for debugging purposes with the given message template.
        /// A timer is a measure of the number of milliseconds elapsed between a start and end time, for example the
        /// time to complete rendering of a web page for a user. Valid timer values are in the range [0, 2^64^).
        /// </summary>
        /// <example>
        ///
        /// using(Log.Timer("Load the Companies", warnIfExceeds: TimeSpan.FromSeconds(1))
        /// {
        ///     LoadTheCompanies();
        /// }
        ///
        /// </example>
        public static IDisposable Timer(this ILog logger, String messageTemplate, params object[] context)
        {
            return new Timing(logger, null, messageTemplate, context);
        }

        /// <summary>
        /// Track the execution time of a particular block of code for debugging purposes with the given message template.
        /// A timer is a measure of the number of milliseconds elapsed between a start and end time, for example the
        /// time to complete rendering of a web page for a user. Valid timer values are in the range [0, 2^64^).
        ///
        /// You can raise warning level events if a timeout is exceeded by providing a threshold timespan.
        /// </summary>
        /// <example>
        ///
        /// using(Log.Timer("Load the Companies", warnIfExceeds: TimeSpan.FromSeconds(1))
        /// {
        ///     LoadTheCompanies();
        /// }
        ///
        /// </example>
        public static IDisposable Timer(this ILog logger, String messageTemplate, TimeSpan warnIfExceeds, params object[] context)
        {
            return new Timing(logger, warnIfExceeds, messageTemplate, context);
        }

        //public static void Gauge(this ILog logger, String name, String unit_of_measure, Func<long> value)
        //{
        //
        //}

        // TODO Counter Increment/Decrement/Reset?

        /// <summary>
        /// An implementation of the metrics gathering class that will track the amount of time the given action takes to complete.
        /// </summary>
        private sealed class Timing : IDisposable
        {
            private readonly ILog logger;
            private readonly TimeSpan? warnIfExceeds;
            private readonly string messageTemplate;
            private readonly Stopwatch stopWatch;
            private readonly object[] context;

            public Timing(ILog logger, TimeSpan? warnIfExceeds, string messageTemplate, params object[] context)
            {
                this.logger = logger;
                this.warnIfExceeds = warnIfExceeds;
                this.messageTemplate = messageTemplate;
                this.context = context;

                this.stopWatch = Stopwatch.StartNew();
            }

            public void Dispose()
            {
                this.stopWatch.Stop();

                // If we have a warning level set and we've exceeded it, push this up to a warning level

                LogLevel level = warnIfExceeds.HasValue && stopWatch.Elapsed > warnIfExceeds.Value ? LogLevel.Warn : LogLevel.Info;

                // All credit for this goes to https://github.com/nblumhardt/serilog-timings and Nick Blumhardt

                logger
                    .WithContext(Serilog.Core.Constants.SourceContextPropertyName, nameof(Timing))
                    .Log(level, messageTemplate + " in {TotalMilliseconds:0.0} ms", context.Concat(new object[] { stopWatch.Elapsed.TotalMilliseconds }).ToArray());
            }
        }
    }
}