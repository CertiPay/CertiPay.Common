﻿using CertiPay.Common.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace CertiPay.Common.Notifications
{
    /// <summary>
    /// Provide a simple interface for sending email messages, including
    /// filtering blocked emails or preventing test emails from being sent
    /// outside of the company
    /// </summary>
    /// <remarks>
    /// Implementation may be sent into background processing.
    /// </remarks>
    public interface IEmailService : INotificationSender<EmailNotification>
    {
        /// <summary>
        /// Send Message Synchronously
        /// </summary>
        /// <param name="message"></param>
        void Send(MailMessage message);

        /// <summary>
        /// Send message asynchronously
        /// </summary>
        /// <param name="message"></param>
        Task SendAsync(MailMessage message);

        /// <summary>
        /// Send message asynchronously with a specified cancellation token
        /// </summary>
        /// <param name="message"></param>
        /// <param name="token"></param>
        Task SendAsync(MailMessage message, CancellationToken token);
    }

    public class EmailService : IEmailService
    {
        private static readonly ILog Log = LogManager.GetLogger<IEmailService>();

        private readonly SmtpClient smtp;

        /// <summary>
        /// The length of time we'll allow for downloading attachments for email notifications.
        ///
        /// Defaults to one minute.
        /// </summary>
        public static TimeSpan DownloadTimeout { get; set; } = TimeSpan.FromMinutes(1);

        /// <summary>
        /// A list of domains that we will allow emails to go to from outside of the production environment
        /// </summary>
        public static IEnumerable<String> AllowedTestingDomains { get; set; } = new[] { "certipayonline.com", "certipay.com", "certigy.com" };

        /// <summary>
        /// Determines if AllowedTestingDomains will be evaluated when sending emails.
        /// </summary>
        /// <remarks>Enabled by default in all environments except production.</remarks>
        public static Boolean AllowedTestingDomainsEnabled { get; set; } = !EnvUtil.IsProd;

        public EmailService(SmtpClient smtp)
        {
            this.smtp = smtp;
        }

        public async Task SendAsync(EmailNotification notification)
        {
            await SendAsync(notification, CancellationToken.None);
        }

        public async Task SendAsync(EmailNotification notification, CancellationToken token)
        {
            using (var msg = new MailMessage { })
            {
                // If no address is provided, it will use the default one from the Smtp config

                if (!String.IsNullOrWhiteSpace(notification.FromAddress))
                {
                    msg.From = new MailAddress(notification.FromAddress, notification.FromName ?? notification.FromAddress);
                }

                foreach (var recipient in notification.Recipients)
                {
                    msg.To.Add(recipient);
                }

                foreach (var recipient in notification.CC)
                {
                    msg.CC.Add(recipient);
                }

                foreach (var recipient in notification.BCC)
                {
                    msg.Bcc.Add(recipient);
                }

                msg.Subject = notification.Subject;
                msg.Body = notification.Content;

                msg.IsBodyHtml = (notification.EmailType == EmailNotification.EmailFormat.HTML);

                foreach (var attachment in notification.Attachments)
                {
                    await AttachFile(msg, attachment);
                }

                await SendAsync(msg, token);
            }
        }

        public void Send(MailMessage message)
        {
            using (Log.Timer("EmailService.Send - {@notification}", context: ForLog(message)))
            {
                FilterRecipients(message.To);
                FilterRecipients(message.CC);
                FilterRecipients(message.Bcc);

                smtp.Send(message);
            }
        }

        public async Task SendAsync(MailMessage message)
        {
            await SendAsync(message, CancellationToken.None);
        }

        public async Task SendAsync(MailMessage message, CancellationToken token)
        {
            using (Log.Timer("EmailService.SendAsync - {@notification}", context: ForLog(message)))
            {
                FilterRecipients(message.To);
                FilterRecipients(message.CC);
                FilterRecipients(message.Bcc);

                await smtp.SendMailAsync(message).ConfigureAwait(false);
            }
        }

        public virtual void FilterRecipients(MailAddressCollection addresses)
        {
            if (AllowedTestingDomainsEnabled)
            {
                // This is so we don't accidentally send customers emails from non-prod environments

                var to_remove = from email in addresses
                                where !AllowedTestingDomains.Contains(email.Host.ToLower())
                                select email;

                foreach (var address in to_remove.ToList())
                {
                    Log.Info("Filtering address {0} from email from test environment", address);
                    addresses.Remove(address);
                }
            }

            // TODO Check blacklisted email addresses?
        }

        internal async Task AttachFile(MailMessage msg, EmailNotification.Attachment attachment)
        {
            if (String.IsNullOrWhiteSpace(attachment.Filename))
            {
                throw new ArgumentException("Attachment is missing a filename");
            }

            if (String.IsNullOrWhiteSpace(attachment.Uri) && String.IsNullOrWhiteSpace(attachment.Content))
            {
                throw new ArgumentException("Attachment does not have content");
            }

            using (Log.Timer("Attaching {@attachment} for {@msg}", new { attachment.Filename, attachment.Uri }, ForLog(msg)))
            {
                byte[] data = new byte[] { };

                if (!String.IsNullOrWhiteSpace(attachment.Content))
                {
                    // Attach the content decoded from Base64

                    data = Convert.FromBase64String(attachment.Content);
                }

                if (!String.IsNullOrWhiteSpace(attachment.Uri))
                {
                    using (HttpClient client = new HttpClient(new HttpClientHandler { AllowAutoRedirect = true }) { Timeout = DownloadTimeout })
                    {
                        // Attach the file downloaded

                        data = await client.GetByteArrayAsync(attachment.Uri).ConfigureAwait(false);
                    }
                }

                // MailMessage disposes of the attachment stream when it disposes

                msg.Attachments.Add(new Attachment(new MemoryStream(data), attachment.Filename));
            }
        }

        private static object ForLog(MailMessage msg)
        {
            return new
            {
                To = String.Join(",", msg.To.Select(_ => _.Address)),
                CC = String.Join(",", msg.CC.Select(_ => _.Address)),
                Bcc = String.Join(",", msg.Bcc.Select(_ => _.Address)),
                msg.From,
                msg.Subject
            };
        }
    }
}