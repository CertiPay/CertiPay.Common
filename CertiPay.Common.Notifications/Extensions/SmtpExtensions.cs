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
        /// https://gist.github.com/mattbenic/400e3c039ab8ea3e33aa
        /// https://msdn.microsoft.com/en-us/library/system.net.mail.smtpclient.sendasynccancel(v=vs.110).aspx
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

            if (!token.IsCancellationRequested)
                using (var reg = token.Register(cancelSend))
                {
                    await client.SendMailAsync(message);
                }
        }
    }
}
