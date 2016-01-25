using CertiPay.Common.Logging;
using CertiPay.Common.WorkQueue;
using System.Threading;
using System.Threading.Tasks;

namespace CertiPay.Common.Notifications
{
    /// <summary>
    /// Sends notifications to the background worker queue for async processing and retries
    /// </summary>
    public partial class QueuedSender :
        INotificationSender<SMSNotification>,
        INotificationSender<EmailNotification>
    {
        private static readonly ILog Log = LogManager.GetLogger<QueuedSender>();

        private readonly IQueueManager _queue;

        public QueuedSender(IQueueManager queue)
        {
            this._queue = queue;
        }

        public async Task SendAsync(EmailNotification notification)
        {
            await SendAsync(notification, CancellationToken.None); 
        }
        public async Task SendAsync(EmailNotification notification, CancellationToken token)
        {
            using (Log.Timer("QueuedSender.SendAsync", context: notification))
            {
                await _queue.Enqueue(EmailNotification.QueueName, notification);
            }
        }
        public async Task SendAsync(SMSNotification notification)
        {
            await SendAsync(notification, CancellationToken.None);
        }
        public async Task SendAsync(SMSNotification notification, CancellationToken token)
        {
            using (Log.Timer("QueuedSender.SendAsync", context: notification))
            {
                await _queue.Enqueue(SMSNotification.QueueName, notification);
            }
        }
    }
}