using CertiPay.Common.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace CertiPay.Common.Notifications.Notifications
{
    /// <summary>
    /// An implementation of INotificationSender<EmailNotification> and INotificationSender<SMSNotification>
    /// that just logs the full notification out to the ILog implementation for debugging.
    /// This is useful when running locally, when we don't want actual notifications to get sent out.
    /// </summary>
    public sealed class NoOpNotificationSender : INotificationSender<EmailNotification>, INotificationSender<SMSNotification>
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public Task SendAsync(EmailNotification notification)
        {
			return SendAsync(notification, CancellationToken.None);
        }

        public Task SendAsync(EmailNotification notification, CancellationToken token)
        {
            Log.Info("NoOpNotificationSender not sending {@notification}", notification);
            return Task.FromResult(0);
        }

        public Task SendAsync(SMSNotification notification)
        {
            return SendAsync(notification, CancellationToken.None);
        }

        public Task SendAsync(SMSNotification notification, CancellationToken token)
        {
            Log.Info("NoOpNotificationSender not sending {@notification}", notification);
            return Task.FromResult(0);
        }
    }
}