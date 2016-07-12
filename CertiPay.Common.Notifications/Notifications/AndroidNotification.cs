using System;

namespace CertiPay.Common.Notifications.Notifications
{
    public class AndroidNotification : Notification
    {
        public static string QueueName { get; } = "AndroidNotifications";

        // Message => Content

        /// <summary>
        /// The subject line of the email
        /// </summary>
        public String Title { get; set; }

        // TODO Android specific properties? Image, Sound, Action Button, Picture, Priority
    }
}