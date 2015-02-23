﻿using CertiPay.Common.Logging;
using CertiPay.Common.WorkQueue;
using System.Threading.Tasks;

namespace CertiPay.Common.Notifications
{
    /// <summary>
    /// Sends notifications to the background worker queue for async processing and retries
    /// </summary>
    public class QueuedSender :
        //IIdentityMessageService,
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
            Log.Info("Sending Notification via Queue {Queue}: {@Notification}", EmailNotification.QueueName, notification);

            await _queue.Enqueue(EmailNotification.QueueName, notification);
        }

        public async Task SendAsync(SMSNotification notification)
        {
            Log.Info("Sending Notification via Queue {Queue}: {@Notification}", SMSNotification.QueueName, notification);

            await _queue.Enqueue(SMSNotification.QueueName, notification);
        }

        //public async Task SendAsync(IdentityMessage message)
        //{
        //    // TODO Add logging

        //    // TODO Might need a sanity check in here to make sure something doesn't go to the wrong format

        //    if (String.IsNullOrWhiteSpace(message.Subject))
        //    {
        //        await SendAsync(new SMSNotification
        //        {
        //            Recipients = new[]
        //            {
        //                message.Destination
        //            },
        //            Content = message.Body
        //        });
        //    }
        //    else
        //    {
        //        await SendAsync(new EmailNotification
        //        {
        //            FromAddress = EmailService.NO_REPLY_ADDR,
        //            Recipients = new[]
        //            {
        //                message.Destination
        //            },
        //            Subject = message.Subject,
        //            Content = message.Body
        //        });
        //    }
        //}
    }
}