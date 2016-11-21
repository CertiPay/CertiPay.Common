using CertiPay.Common.Logging;
using CertiPay.Common.Redis;
using CertiPay.Common.Testing;
using CertiPay.Common.WorkQueue;
using NUnit.Framework;
using System;
using System.Linq;

namespace CertiPay.Services.Redis
{
    public class RedisQueueManagerTests
    {
        private static readonly ILog Log = LogManager.GetLogger<RedisQueueManagerTests>();

        private String queueName = "TestQueue";
        private String message = "This is a test!";

        private RedisConnection conn;
        private RedisQueueManager queue;

        [SetUp]
        public void SetUp()
        {
            // Arrange

            conn = new RedisConnection();
            queue = new RedisQueueManager(conn);
        }

        [Test, Integration]
        public void Should_Enqueue_Items()
        {
            // Act

            AddTestMessages(count: 1, queueName: queueName);

            var item = queue.GetWork<TestMessage>(queueName).Result;

            // Assert
            Assert.AreEqual(message, item.Message);
            Assert.IsTrue(item.Created > DateTime.UtcNow.AddSeconds(-2));
        }

        [Test, Integration]
        public void Should_Enqueue_Items_Performance_Test()
        {
            // Arrange

            var range = 1000;

            var queueNames = new[] { "TestQueue1", "TestQueue2", "TestQueue3", "TestQueue4", "TestQueue5" };

            using (Log.Timer("Iterating through all queues"))
            {
                foreach (var queueName in queueNames)
                {
                    using (Log.Timer("Loading into queue", queueName))
                    {
                        // Act

                        AddTestMessages(count: range, queueName: queueName);
                    }
                }
                using (Log.Timer("Loading from queue", queueName))
                {
                    foreach (var msg in queue.GetConsumingEnumerable<TestMessage>(queueName))
                    {
                        Assert.AreEqual(message, msg.Message);
                        Assert.AreEqual(msg.Created.Date, DateTime.UtcNow.Date);

                        if (msg.Created.Ticks % 2 == 0)
                        {
                            queue.MarkComplete(queueName, new CompletedWorkItem<TestMessage> { WorkItem = msg });
                        }
                        else
                        {
                            queue.MarkFailed(queueName, new FailedWorkItem<TestMessage> { WorkItem = msg, ErrorMessage = "Shitz broke!" });
                        }
                    }
                }
            }
        }

        [Test, Integration]
        public void Should_Consume_Until_Empty()
        {
            // Act

            var range = 1000;

            AddTestMessages(count: range, queueName: queueName);

            int count = 0;

            // Assert - kind of

            foreach (var item in queue.GetConsumingEnumerable<TestMessage>(queueName))
            {
                Assert.AreEqual(message, item.Message);

                count++;
            }

            Assert.AreEqual(range, count);
        }

        [Test, Integration]
        public void Should_Batch_Work_Dequeue()
        {
            var range = 1000;

            AddTestMessages(count: range, queueName: queueName);

            var batch_size = 5;

            var batches = (range / batch_size);

            var batch_count = 0;

            foreach (var batch_id in Enumerable.Range(0, batches))
            {
                var batch = queue.GetConsumingEnumerable<TestMessage>(queueName).Take(batch_size).ToList();

                Assert.AreEqual(batch_size, batch.Count);

                Assert.IsTrue(batch.All(m => m.Message == message));

                batch_count++;
            }

            Assert.AreEqual(batches, batch_count);
        }

        [Test, Integration]
        public void Should_Batch_Work_Uneven_Batch()
        {
            var range = 13;

            AddTestMessages(count: range, queueName: queueName);

            var batch_size = 5;

            var batches = (int)Math.Ceiling((double)range / batch_size);

            var batch_count = 0;

            foreach (var batch_id in Enumerable.Range(0, batches))
            {
                var batch = queue.GetConsumingEnumerable<TestMessage>(queueName).Take(batch_size).ToList();

                Assert.IsTrue(batch.All(m => m.Message == message));
                Assert.IsTrue(batch.Count == 3 || batch.Count == 5);

                batch_count++;
            }

            Assert.AreEqual(batches, batch_count);
        }

        private void AddTestMessages(int count, string queueName)
        {
            queue.Purge(queueName);

            foreach (var i in Enumerable.Range(1, count))
            {
                queue.Enqueue(queueName, new TestMessage
                {
                    Message = message,
                    Active = false,
                    ID = Guid.NewGuid()
                });
            };
        }

        public class TestMessage
        {
            public Guid ID { get; set; }

            public String Message { get; set; }

            public Boolean Active { get; set; }

            public DateTime Created = DateTime.UtcNow;
        }
    }
}