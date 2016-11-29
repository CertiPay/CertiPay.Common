using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace CertiPay.Common
{
    public static class ExtensionMethods
    {
        private static readonly Encoding DefaultEncoding = Encoding.UTF8;

        private static readonly Regex alphanumeric = new Regex("[^a-zA-Z0-9]");

        /// <summary>
        /// Removes any non-alphanumeric characters from the string
        /// </summary>
        public static String StripNonAlphanumeric(this String s)
        {
            return alphanumeric.Replace(s, String.Empty);
        }

        /// <summary>
        /// Trims the string of any whitestace and leaves null if there is no content.
        /// </summary>
        public static String TrimToNull(this String s)
        {
            return String.IsNullOrWhiteSpace(s) ? null : s.Trim();
        }

        /// <summary>
        /// Returns the given string as a Stream, optionally specifiying the encoding (Default UTF8)
        /// </summary>
        public static Stream Streamify(this string theString, Encoding encoding = null)
        {
            return new MemoryStream((encoding ?? DefaultEncoding).GetBytes(theString));
        }

        /// <summary>
        /// Returns the stream as a string, optionally specifiying the encoding (Default UTF8)
        /// </summary>
        public static string Stringify(this Stream theStream, Encoding encoding = null)
        {
            using (var reader = new StreamReader(theStream, encoding ?? DefaultEncoding))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Returns the byte array as  GZip-format compressed data
        /// </summary>
        public static byte[] Compress(this byte[] data)
        {
            using (var writeStream = new MemoryStream { })
            using (var zipStream = new GZipStream(writeStream, CompressionMode.Compress))
            {
                zipStream.Write(data, 0, data.Length);
                zipStream.Close();

                return writeStream.ToArray();
            }
        }

        /// <summary>
        /// Returns the GZip-format compressed byte array after decompressing
        /// </summary>
        /// <remarks>
        /// This method utilizes the built-in Stream.CopyTo, so the default system buffer size of 4096 is used.
        /// </remarks>
        public static byte[] Decompress(this byte[] data)
        {
            using (var readStream = new MemoryStream(data))
            using (var zipStream = new GZipStream(readStream, CompressionMode.Decompress))
            using (var writeStream = new MemoryStream { })
            {
                zipStream.CopyTo(writeStream);

                return writeStream.ToArray();
            }
        }

        /// <summary>
        /// Returns a substring of the first N characters
        /// </summary>
        public static String Truncate(this String s, int length)
        {
            if (s.Length > length)
            {
                s = s.Substring(0, length);
            }

            return s;
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

            if (fi != null)
            {
                var attributes = fi.GetCustomAttributes<DisplayAttribute>();

                if (attributes != null && attributes.Any())
                {
                    return selector.Invoke(attributes.First());
                }
            }

            return val.ToString();
        }
    }
}