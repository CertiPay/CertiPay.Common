using CertiPay.Common.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Configuration;

namespace CertiPay.Common
{
    /// <summary>
    /// A mechanism for determining the current execution environment based on a configuration setting.
    /// Useful for executing different code paths if running locally our outside of production.
    ///
    /// This requires that ConfigurationManager.AppSettings["Environment"] is set to Local/Test/Staging/Production
    /// </summary>
    public static class EnvUtil
    {
        private const String ENV_FLAG = "ASPNETCORE_ENVIRONMENT";

        private const String CONFIG_FLAG = "Environment";

        [Flags]
        public enum Environment : byte
        {
            // ASPNET Core uses Development, Staging, and Production
            // For our purposes, we're going to consider Local and Development the same

            // ToString() on an enum containing multiple definitions with the same value will 
            // concatenate the values together. Provide an alternative means to display the value

            [Display(Name = "Local")]
            Local = 0,
            [Display(Name = "Development")]
            Development = Local,
            Test = 1 << 0,
            Staging = 1 << 1,
            Production = 1 << 2
        }

        /// <summary>
        /// Returns the current executing environment. Defaults to Local if no value is set in the config
        /// </summary>
        public static Environment Current { get { return current.Value; } }

        /// <summary>
        /// Returns true if the current environment is marked as Environment.Local
        /// </summary>
        public static Boolean IsLocal { get { return Current.HasFlag(Environment.Local); } }

        /// <summary>
        /// Returns true if the current environment is marked as Environment.Development
        /// </summary>
        public static Boolean IsDevelopment { get { return Current.HasFlag(Environment.Development); } }

        /// <summary>
        /// Returns true if the current environment is marked as Environment.Test
        /// </summary>
        public static Boolean IsTest { get { return Current.HasFlag(Environment.Test); } }

        /// <summary>
        /// Returns true if the current environment is marked as Environment.Staging
        /// </summary>
        public static Boolean IsStaging { get { return Current.HasFlag(Environment.Staging); } }

        /// <summary>
        /// Returns true if the current environment is marked as Environment.Production
        /// </summary>
        public static Boolean IsProd { get { return Current.HasFlag(Environment.Production); } }

        private static readonly Lazy<Environment> current = new Lazy<Environment>(() =>
        {
            Environment environment = Environment.Local;

            // First check the app config, since we might be specifying it there

            if (!Enum.TryParse(ConfigurationManager.AppSettings[CONFIG_FLAG], ignoreCase: true, result: out environment))
            {
                // Couldn't parse the config, try the environment variables a la ASPNET Core

                if (!Enum.TryParse(System.Environment.GetEnvironmentVariable(ENV_FLAG), ignoreCase: true, result: out environment))
                {
                    // Couldn't parse from the environment, fall back to local and warn the user
                    // Actually, I can't do that here, since EnvUtil.Current is used in the log setup, so it's a vicious circle

                    // LogManager.GetLogger(nameof(EnvUtil)).Warn("Could not parse environment configuration, defaulting to LOCAL");
                }
            }

            return environment;
        });
    }
}