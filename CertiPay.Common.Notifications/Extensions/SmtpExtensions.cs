using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CertiPay.Common.Notifications.Extensions
{
    public static class SmtpExtensions
    {
        /// <summary>
        /// Extension method to have SmtpClient's SendMailAsync respond to a CancellationToken
        /// </summary>
        public static async Task SendMailAsync(
            this SmtpClient client,
            MailMessage message,
            CancellationToken token)
        {
            Action cancelSend = () =>
            {
                client.SendAsyncCancel();
            };
            using (var reg = token.Register(cancelSend))
            {
                await client.SendMailAsync(message);
            }
        }
    }
}
