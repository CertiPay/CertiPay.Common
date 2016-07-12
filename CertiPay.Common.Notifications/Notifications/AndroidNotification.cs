namespace CertiPay.Common.Notifications.Notifications
{
    public class AndroidNotification : Notification
    {
        public static string QueueName { get { return "AndroidNotifications"; } }

        // TODO Android specific properties? Title, Image, Sound, Action Button, Picture, Priority

        // Message => Content
    }
}