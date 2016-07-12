using System;

namespace CertiPay.Common.Notifications
{
    /// <summary>
    /// Describes a notification sent to an Android device via Google Cloud Messaging
    /// </summary>
    public class AndroidNotification : Notification
    {
        public static string QueueName { get; } = "AndroidNotifications";

        /// <summary>
        /// The subject line of the notification
        /// </summary>
        public String Title { get; set; }

        /// <summary>
        /// Maximum lifespan of the message, from 0 to 4 weeks, after which delivery attempts will expire.
        /// Setting this to 0 seconds will prevent GCM from throttling the "now or never" message.
        ///
        /// GCM defaults this to 4 weeks.
        /// </summary>
        public TimeSpan? TimeToLive { get; set; }

        /// <summary>
        /// Set high priority only if the message is time-critical and requires the user’s
        /// immediate interaction, and beware that setting your messages to high priority contributes
        /// more to battery drain compared to normal priority messages.
        /// </summary>
        public Boolean HighPriority { get; set; } = false;
    }
}