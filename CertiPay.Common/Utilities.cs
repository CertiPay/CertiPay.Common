namespace CertiPay.Common
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Transactions;

    public static class Utilities
    {
        /// <summary>
        /// Starts a new transaction scope with the given isolation level
        /// </summary>
        /// <returns></returns>
        public static TransactionScope StartTrx(IsolationLevel Isolation = IsolationLevel.ReadUncommitted)
        {
            return new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = Isolation });
        }

        /// <summary>
        /// Performs manual validation of the object using the data annotations, returning
        /// true if it passes and provides a list of results from the validation
        /// </summary>
        /// <remarks>
        /// Make sure that you have any necessary resources loaded (i.e. CertiPay.Resources) for error messages
        /// or else this will always return true!!!
        /// </remarks>
        public static Boolean TryValidate(object @object, ICollection<ValidationResult> results)
        {
            var context = new ValidationContext(@object, serviceProvider: null, items: null);

            return Validator.TryValidateObject(@object, context, results, validateAllProperties: true);
        }

        /// <summary>
        /// Removes invalid characters from filenames and replaces them with the given character
        /// </summary>
        public static String FixFilename(String filename, String replacement = "")
        {
            return Path.GetInvalidFileNameChars().Aggregate(filename, (current, c) => current.Replace(c.ToString(), replacement));
        }

        /// <summary>
        /// Returns the assembly version of the assembly formatted as Major.Minor (build Build#)
        /// </summary>
        public static String AssemblyVersion<T>()
        {
            var version =
                typeof(T)
                .Assembly
                .GetName()
                .Version;

            return String.Format("{0}.{1} (build {2})", version.Major, version.Minor, version.Build);
        }

        /// <summary>
        /// Returns the assembly informational version of the type, or assembly version if not available
        /// </summary>
        public static String Version<T>()
        {
            var attribute =
                typeof(T)
                .Assembly
                .GetCustomAttributes(false)
                .OfType<AssemblyInformationalVersionAttribute>()
                .FirstOrDefault();

            return attribute == null ? AssemblyVersion<T>() : attribute.InformationalVersion;
        }
    }
}