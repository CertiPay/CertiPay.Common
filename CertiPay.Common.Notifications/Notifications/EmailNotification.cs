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
        /// Who the email notification should be FROM
        /// </summary>
        public String FromAddress { get; set; }

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
        /// A file may be attached to the email notification by providing a URL to download
        /// the file (will be downloaded by the sending process) and a filename
        /// </summary>
        public class Attachment
        {
            public String Filename { get; set; }

            public String Uri { get; set; }
        }
    }
}