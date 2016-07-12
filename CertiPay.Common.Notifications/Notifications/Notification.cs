using System;
using System.Collections.Generic;

namespace CertiPay.Common.Notifications
{
    /// <summary>
    /// Describes the base notification content required to send
    /// </summary>
    public class Notification
    {
        /// <summary>
        /// The content of the message
        /// </summary>
        public virtual String Content { get; set; } = String.Empty;

        /// <summary>
        /// A list of recipients for the notification, whether emails, phone numbers, or device identifiers
        /// </summary>
        public virtual ICollection<String> Recipients { get; set; } = new List<String>();
    }
}