﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace CertiPay.Common
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Trims the string of any whitestace and leaves null if there is no content.
        /// </summary>
        public static String TrimToNull(this String s)
        {
            return String.IsNullOrWhiteSpace(s) ? null : s.Trim();
        }

        /// <summary>
        /// Returns the display name from the display attribute on the enumeration, if available.
        /// Otherwise returns the ToString() value.
        /// </summary>
        public static string DisplayName(this Enum val)
        {
            return val.Display(e => e.GetName());
        }

        /// <summary>
        /// Returns the short name from the display attribute on the enumeration, if available.
        /// Otherwise returns the ToString() value.
        /// </summary>
        public static string ShortName(this Enum val)
        {
            return val.Display(e => e.GetShortName());
        }

        /// <summary>
        /// Returns the description from the display attribute on the enumeration, if available.
        /// Otherwise returns the ToString() value.
        /// </summary>
        public static string Description(this Enum val)
        {
            return val.Display(e => e.GetDescription());
        }

        private static String Display(this Enum val, Func<DisplayAttribute, String> selector)
        {
            FieldInfo fi = val.GetType().GetField(val.ToString());

            var attributes = fi.GetCustomAttributes<DisplayAttribute>();

            if (attributes != null && attributes.Any())
            {
                return selector.Invoke(attributes.First());
            }

            return val.ToString();
        }
    }
}