namespace CertiPay.Common.Logging
{
    using CertiPay.Common;
    using System;
    using System.Configuration;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// A class to handle the configuration and routing of log entries to various sinks, so that
    /// an application is agnostic to it's destination.
    /// </summary>
    public class LogManager
    {
        static LogManager()
        {
            // Note: These properties are statically configured here, but can be adjusted via their
            // respective set properties by an application for special use cases.

            ApplicationName = ConfigurationManager.AppSettings["ApplicationName"].TrimToNull() ?? "Unknown";

            LogPathFormat = Path.Combine(GetLogFolder(), "{Date}.log");

            var logLevelConfig = ConfigurationManager.AppSettings["LogLevel"].TrimToNull() ?? "Info";

            LogLevel = (LogLevel)Enum.Parse(typeof(LogLevel), logLevelConfig, true);

            Version = Utilities.Version<LogManager>();
        }

        /// <summary>
        /// String describing the location of the log files, with {Date} in the place
        /// of the file date. E.g. "Logs\myapp-{Date}.log" will result in log files such
        /// as "Logs\myapp-2013-10-20.log", "Logs\myapp-2013-10-21.log" and so on..
        ///
        /// Defaults to c:\Logs\{Environment}\{Application}\{Date}.log
        /// </summary>
        public static String LogPathFormat { get; set; }

        /// <summary>
        /// The name of the application being logged for.
        ///
        /// Pulls from AppSettings["Application"], defaults to Unknown
        /// </summary>
        public static String ApplicationName { get; set; }

        /// <summary>
        /// The application version number, included in the logs for debugging purposes.
        ///
        /// Defaults to the version of CertiPay.Common package
        /// </summary>
        public static String Version { get; set; }

        /// <summary>
        /// Adjusts the logging level for the entire log system.
        ///
        /// Pulls from AppSettings["LogLevel"], defaults to Info.
        /// Possible values are Verbose, Debug, Info, Warn, Error, Fatal
        /// </summary>
        public static LogLevel LogLevel { get; private set; }

        /// <summary>
        /// Returns a configured logger for the current class
        /// </summary>
        public static ILog GetCurrentClassLogger()
        {
            // The previous implementation from the RavenDb Logging abstractions broke in some unforseen situations
            // This implementation is from the NLog LogManager and appears more robust

            string className;
            Type declaringType;
            int framesToSkip = 1;

            do
            {
                StackFrame frame = new StackFrame(framesToSkip, false);

                MethodBase method = frame.GetMethod();

                declaringType = method.DeclaringType;

                if (declaringType == null)
                {
                    className = method.Name;
                    break;
                }

                framesToSkip++;

                className = declaringType.FullName;
            }
            while (declaringType.Module.Name.Equals("mscorlib.dll", StringComparison.OrdinalIgnoreCase));

            return GetLogger(className);
        }

        /// <summary>
        /// Returns a configured logger for the given type
        /// </summary>
        public static ILog GetLogger<T>()
        {
            return GetLogger(typeof(T));
        }

        /// <summary>
        /// Returns a configured logger for the given type
        /// </summary>
        public static ILog GetLogger(Type type)
        {
            return GetLogger(type.FullName);
        }

        /// <summary>
        /// Returns a configured logger for the given key
        /// </summary>
        public static ILog GetLogger(String key)
        {
            return new SerilogManager(key);
        }

        private static String GetLogRootPath()
        {
            const string key = "LogsPath";

            if (!String.IsNullOrWhiteSpace(ConfigurationManager.AppSettings[key]))
            {
                return ConfigurationManager.AppSettings[key];
            }

            if (!String.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.Machine)))
            {
                return Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.Machine);
            }

            if (!String.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.User)))
            {
                return Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.User);
            }

            return @"c:\Logs";
        }

        private static String GetLogFolder()
        {
            return Path.Combine(GetLogRootPath(), EnvUtil.Current.ToString(), ApplicationName);
        }
    }
}