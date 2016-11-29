using CertiPay.Common.WorkQueue;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CertiPay.Common.Redis
{
    /// <summary>
    /// An implementation of the IQueueManager that utilizes Redis lists for it's backend
    /// </summary>
    public class RedisQueueManager : IQueueManager
    {
        private const String QueueSet = "QueueNames";

        private readonly RedisConnection _connection;

        public RedisQueueManager(RedisConnection connection)
        {
            this._connection = connection;
        }

        public async Task<IEnumerable<Queue>> GetQueues()
        {
            // Kind of ugly, need to test how fast this is. Might be able to do it more async/parallel

            var client = _connection.GetClient();

            var queueNames = await client.SetMembersAsync(QueueSet).ConfigureAwait(false);

            return from queue in queueNames
                   select new Queue
                   {
                       Name = queue,
                       Queued = client.ListLength(GetQueue(queue, Queue.QueuedNamespace)),
                       Processed = client.ListLength(GetQueue(queue, Queue.ProcessedNamespace)),
                       Failed = client.ListLength(GetQueue(queue, Queue.FailedNamespace))
                   };
        }

        public async Task<T> GetWork<T>(String queueName) where T : class
        {
            // Pop an item off the queue

            var queue = GetQueue(queueName, Queue.QueuedNamespace);

            var value = await _connection.GetClient().ListRightPopAsync(queue).ConfigureAwait(false);

            if (String.IsNullOrWhiteSpace(value)) return default(T);

            return JsonConvert.DeserializeObject<T>(value);
        }

        public IEnumerable<T> GetConsumingEnumerable<T>(String queueName) where T : class
        {
            var queue = GetQueue(queueName, Queue.QueuedNamespace);

            T t = default(T);

            while ((t = GetWork<T>(queue).Result) != null)
            {
                yield return t;
            }
        }

        public async Task<IEnumerable<T>> GetAll<T>(String queueName, int page = 1, int itemsPerPage = 20) where T : class
        {
            // Get all items in the queue but do not remove them

            var queue = GetQueue(queueName);

            int start = --page * itemsPerPage;
            int stop = start + itemsPerPage;

            var values = await _connection.GetClient().ListRangeAsync(queue, start, stop).ConfigureAwait(false);

            return from value in values select JsonConvert.DeserializeObject<T>(value);
        }

        public async Task Enqueue<T>(String queueName, T t) where T : class
        {
            // Add an item to the queue

            var queue = GetQueue(queueName, Queue.QueuedNamespace);

            var json = JsonConvert.SerializeObject(t);

            await _connection.GetClient().ListLeftPushAsync(queue, json).ConfigureAwait(false);

            await TrackQueueName(queueName);
        }

        public async Task MarkComplete<T>(String queueName, CompletedWorkItem<T> t) where T : class
        {
            var queue = GetQueue(queueName, Queue.ProcessedNamespace);

            var json = JsonConvert.SerializeObject(t);

            await _connection.GetClient().ListLeftPushAsync(queue, json).ConfigureAwait(false);
        }

        public async Task MarkFailed<T>(String queueName, FailedWorkItem<T> t) where T : class
        {
            var queue = GetQueue(queueName, Queue.FailedNamespace);

            var json = JsonConvert.SerializeObject(t);

            await _connection.GetClient().ListLeftPushAsync(queue, json).ConfigureAwait(false);
        }

        public async Task Purge(String queueName)
        {
            // Remove all of the items in the given queue

            var queue = GetQueue(queueName);

            await _connection.GetClient().KeyDeleteAsync(queue).ConfigureAwait(false);
        }

        private async Task TrackQueueName(String queueName)
        {
            // Keep a list of work queues for faster reference when building admin page
            // Assuming the given queue name doesn't have any namespacing...

            await _connection.GetClient().SetAddAsync(QueueSet, queueName).ConfigureAwait(false);
        }

        private String GetQueue(String queueName, String queueNamespace = Queue.QueuedNamespace)
        {
            // Check if it already has the namespace added, otherwise add it

            var namespaces = new[] { Queue.QueuedNamespace, Queue.ProcessedNamespace, Queue.FailedNamespace };

            if (namespaces.Any(ns => queueName.StartsWith(ns))) return queueName;

            return String.Format("{0}{1}", queueNamespace, queueName);
        }
    }
}