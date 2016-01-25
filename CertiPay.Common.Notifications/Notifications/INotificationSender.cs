using System.Threading;
using System.Threading.Tasks;

namespace CertiPay.Common.Notifications
{
    public interface INotificationSender<in T>
    {
        /// <summary>
        /// Send a notification asyncronously
        /// </summary>
        Task SendAsync(T notification);

        /// <summary>
        /// Send a notification asyncronously with a specified cancellation token
        /// </summary>
        Task SendAsync(T notification, CancellationToken token);
    }
}