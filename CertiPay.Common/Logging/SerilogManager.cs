namespace CertiPay.Common.Logging
{
    using Serilog;
    using Serilog.Events;
    using SerilogWeb.Classic;
    using SerilogWeb.Classic.Enrichers;
    using System;

    internal class SerilogManager : ILog
    {
        private static readonly Lazy<ILogger> logger = new Lazy<ILogger>(() =>
        {
            // Set the defaults so that form data will be logged as info on errors
            // The default for request logging is Information already

            ApplicationLifecycleModule.RequestLoggingLevel = LogEventLevel.Information;
            ApplicationLifecycleModule.FormDataLoggingLevel = LogEventLevel.Information;
            ApplicationLifecycleModule.LogPostedFormData = LogPostedFormDataOption.OnlyOnError;

            // Provide a default rolling file and console configuration for Serilog
            // User can configure application settings to add on properties or sinks
            // This ensures that the configuration is done once before use

            return
                Serilog.Log.Logger =
                    new LoggerConfiguration()
                        .ReadFrom.AppSettings()

                        .MinimumLevel.Is(GetLevel(LogManager.LogLevel))

                        .Enrich.FromLogContext()
                        .Enrich.WithMachineName()

                        // Note: These properties come from the application config file
                        .Enrich.WithProperty("ApplicationName", LogManager.ApplicationName)
                        .Enrich.WithProperty("Version", LogManager.Version)
                        .Enrich.WithProperty("Environment", EnvUtil.Current)

                        // Note: These enrichers grab info off of the HttpRequest.Context if it's available, otherwise are no-ops
                        // Further, note that these become available via the log template but are not automatically included in the rolling file output
                        .Enrich.With<HttpRequestIdEnricher>()
                        .Enrich.With<HttpSessionIdEnricher>()
                        .Enrich.With<HttpRequestRawUrlEnricher>()
                        .Enrich.With<HttpRequestUrlReferrerEnricher>()
                        .Enrich.With<UserNameEnricher>()
                        .Enrich.With<HttpRequestClientHostIPEnricher>()
                        .Enrich.With<HttpRequestUserAgentEnricher>()

                        .WriteTo.ColoredConsole()

#if DEBUG
                        .WriteTo.Sink(new Serilog.Sinks.RollingFile.RollingFileSink(@"c:\logs\out.json", new Serilog.Formatting.Json.JsonFormatter { }, null, null, null))
#endif

                        .WriteTo.RollingFile(
                            // Environment, ApplicationName, and date are already in the folder\file name
                            outputTemplate: "{Timestamp:HH:mm:ss.fff} [{Level}] [{Logger}] {Message}{NewLine}{Exception}",
                            pathFormat: LogManager.LogFilePath
                        )

                        .CreateLogger();
        });

        private readonly ILogger _logger;

        internal SerilogManager(String key)
        {
            _logger = logger.Value.ForContext("Logger", key);
        }

        internal SerilogManager(ILogger logger)
        {
            _logger = logger;
        }

        public void Log(LogLevel level, string messageTemplate, params object[] propertyValues)
        {
            _logger.Write(GetLevel(level), messageTemplate, propertyValues);
        }

        public void Log<TException>(LogLevel level, string messageTemplate, TException exception, params object[] propertyValues) where TException : Exception
        {
            _logger.Write(GetLevel(level), exception, messageTemplate, propertyValues);
        }

        public ILog WithContext(string propertyName, object value, Boolean destructureObjects = false)
        {
            // Add the context into the _logger and pass back the ILog interface
            return new SerilogManager(_logger.ForContext(propertyName, value, destructureObjects));
        }

        public IDisposable WithAmbientContext(string name, object value, Boolean destructureObjects = false)
        {
            return Serilog.Context.LogContext.PushProperty(name, value, destructureObjects);
        }

        internal static LogEventLevel GetLevel(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Fatal:
                    return LogEventLevel.Fatal;

                case LogLevel.Error:
                    return LogEventLevel.Error;

                case LogLevel.Warn:
                    return LogEventLevel.Warning;

                case LogLevel.Info:
                    return LogEventLevel.Information;

                case LogLevel.Debug:
                    return LogEventLevel.Debug;

                case LogLevel.Verbose:
                default:
                    return LogEventLevel.Verbose;
            }
        }
    }
}