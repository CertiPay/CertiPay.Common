using CertiPay.Common.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Twilio;

namespace CertiPay.Common.Notifications
{
    /// <summary>
    /// Send an SMS message to the given recipient.
    /// </summary>
    /// <remarks>
    /// Implementation may be sent into background processing.
    /// </remarks>
    public interface ISMSService : INotificationSender<SMSNotification>
    {
        // Task SendAsync(T notification);
    }

    public class SmsService : ISMSService
    {
        private static readonly ILog Log = LogManager.GetLogger<ISMSService>();

        private TwilioRestClient client { get; }

        private readonly String fromNumber;

        public SmsService(TwilioConfig config)
        {
            this.client = new TwilioRestClient(config.AccountSid, config.AuthToken);
            this.fromNumber = config.SourceNumber;
        }

        public Task SendAsync(SMSNotification notification)
        {
            return SendAsync(notification, CancellationToken.None);
        }

        public Task SendAsync(SMSNotification notification, CancellationToken token)
        {
            using (Log.Timer("SMSNotification.SendAsync - @{notification}", context: notification))
            {
                foreach (var recipient in notification.Recipients)
                {
                    if (!token.IsCancellationRequested)
                        client.SendMessage(fromNumber, recipient, notification.Content);
                }

                return Task.FromResult(0);
            }
        }

        public class TwilioConfig
        {
            public String AccountSid { get; set; }

            public String AuthToken { get; set; }

            public String SourceNumber { get; set; }
        }
    }
}