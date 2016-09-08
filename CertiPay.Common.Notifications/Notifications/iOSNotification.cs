using System;

namespace CertiPay.Common.Notifications
{
    /// <summary>
    /// Describes a notification sent to an iOS device via Apple Push Notification Service
    /// </summary>
    public class iOSNotification : Notification
    {
        public static string QueueName { get; } = "iOSNotifications";

        /// <summary>
        /// The subject line of the notification
        /// </summary>
        public String Title { get; set; }

        /// <summary>
        /// Number displayed on the App's icon when a notification arrives.
        /// </summary>
        public string Badge { get; set; }


        /// <summary>
        /// Calls the on('notification') handler in phonegap if set to true, even when the app is
        /// in the background.
        /// </summary>
        public Boolean ContentAvailable { get; set; }

        /// <summary>
        /// Specifies a sound to play on alert, file must be stored locally on Application in root directory
        /// </summary>
        public string Sound { get; set; }
    }
}