﻿using CertiPay.Common.Logging;
using CertiPay.Common.Notifications;
using RestSharp.Serializers;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CertiPay.Notifications
{
    public class ServiceSender : INotificationSender<EmailNotification>, INotificationSender<SMSNotification>, INotificationSender<AndroidNotification>, INotificationSender<iOSNotification>, INotificationSender<HipChatNotification>
    {
        private static readonly ILog Log = LogManager.GetLogger<ServiceSender>();

        public readonly Uri ServiceUri;

        private static TimeSpan Timeout => TimeSpan.FromSeconds(1);

        private readonly JsonSerializer Serializer = new JsonSerializer();

        public ServiceSender(Uri serviceUri)
        {
            this.ServiceUri = serviceUri;
        }

        public ServiceSender(String serviceUri) : this(new Uri(serviceUri))
        {
            // Nothing to do here
        }

        public ServiceSender() : this("http://notifications.certipay.com")
        {
            // Nothing to do here
        }

        public async Task SendAsync(SMSNotification notification)
        {
            await SendAsync(notification, CancellationToken.None);
        }

        public async Task SendAsync(SMSNotification notification, CancellationToken token)
        {
            using (Log.Timer("ServiceSender.SendAsync", warnIfExceeds: Timeout))
            {
                await Post("/SMS", notification, token);
            }
        }

        public async Task SendAsync(EmailNotification notification)
        {
            await SendAsync(notification, CancellationToken.None);
        }

        public async Task SendAsync(EmailNotification notification, CancellationToken token)
        {
            using (Log.Timer("ServiceSender.SendAsync", warnIfExceeds: Timeout))
            {
                await Post("/Emails", notification, token);
            }
        }

        public virtual async Task SendAsync(AndroidNotification notification)
        {
            await SendAsync(notification, CancellationToken.None);
        }

        public virtual async Task SendAsync(AndroidNotification notification, CancellationToken token)
        {
            using (Log.Timer("ServiceSender.SendAsync", warnIfExceeds: Timeout))
            {
                await Post("/Android", notification, token);
            }
        }

        public virtual async Task SendAsync(iOSNotification notification)
        {
            await SendAsync(notification, CancellationToken.None);
        }

        public virtual async Task SendAsync(iOSNotification notification, CancellationToken token)
        {
            using (Log.Timer("ServiceSender.SendAsync", warnIfExceeds: Timeout))
            {
                await Post("/iOS", notification, token);
            }
        }

        [Obsolete(message: "HipChat is no longer in service")]
        public virtual async Task SendAsync(HipChatNotification notification)
        {
            await SendAsync(notification, CancellationToken.None);
        }

        [Obsolete(message: "HipChat is no longer in service")]
        public virtual async Task SendAsync(HipChatNotification notification, CancellationToken token)
        {
            using (Log.Timer("ServiceSender.SendAsync", warnIfExceeds: Timeout))
            {
                await Post("/hipChat", notification, token);
            }
        }

        protected virtual async Task Post<T>(String resource, T t, CancellationToken token)
        {
            var json = Serializer.Serialize(t);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            await GetClient().PostAsync(resource, content, token);
        }

        protected virtual HttpClient GetClient()
        {
            return new HttpClient() { BaseAddress = this.ServiceUri };
        }
    }
}