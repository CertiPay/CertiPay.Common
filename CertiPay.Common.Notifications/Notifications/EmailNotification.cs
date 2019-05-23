using System;
using System.Collections.Generic;

namespace CertiPay.Common.Notifications
{
    /// <summary>
    /// Represents an email notification sent a user, employee, or administrator
    /// </summary>
    public class EmailNotification : Notification
    {
        public static String QueueName { get { return "EmailNotifications"; } }

        /// <summary>
        /// Email address message should be delivered FROM.
        /// </summary>
        public String FromAddress { get; set; }

        /// <summary>
        /// Name of email sender
        /// </summary>
        public String FromName { get; set; }

        /// <summary>
        /// A list of email addresses to CC
        /// </summary>
        public ICollection<String> CC { get; set; } = new List<String>();

        /// <summary>
        /// A list of email addresses to BCC
        /// </summary>
        public ICollection<String> BCC { get; set; } = new List<String>();

        /// <summary>
        /// The subject line of the email
        /// </summary>
        public String Subject { get; set; }

        /// <summary>
        /// Any attachments to the email in the form of URLs to download
        /// </summary>
        public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();

        /// <summary>
        /// Set the email format type to HTML or PlainText, default is HTML.
        /// </summary>
        public EmailFormat EmailType { get; set; } = EmailFormat.HTML;

        /// <summary>
        /// A file may be attached to the email notification
        /// </summary>
        public class Attachment
        {
            /// <summary>
            /// The filename of the attachment
            /// </summary>
            public String Filename { get; set; }

            /// <summary>
            /// If provided, the Base64 encoded content of the attachment
            /// </summary>
            public String Content { get; set; }

            /// <summary>
            /// If provided, an addressable URI from which the service can download the attachment
            /// </summary>
            public String Uri { get; set; }
        }

        public enum EmailFormat
        {
            HTML,
            PlainText
        }
    }
}