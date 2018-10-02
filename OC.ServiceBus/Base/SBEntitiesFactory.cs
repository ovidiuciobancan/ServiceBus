using System;
using Microsoft.Azure.ServiceBus;

namespace OC.ServiceBus
{
    public class SBEntitiesFactory
    {
        private readonly string _connectionString;

        public SBEntitiesFactory(string connectionString)
        {
            if (String.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            _connectionString = connectionString;
        }

        public QueueClient GetQueue(string queueName, ReceiveMode receiveMode = ReceiveMode.PeekLock, RetryPolicy retryPolicy = null)
        {
            return new QueueClient(_connectionString, queueName, receiveMode, retryPolicy);
        }

        public TopicClient GetTopic(string topicName, RetryPolicy retryPolicy = null)
        {
            return new TopicClient(_connectionString, topicName, retryPolicy);
        }

        public SubscriptionClient GetSubscription(string topicName, string subscriptionName, ReceiveMode receiveMode = ReceiveMode.PeekLock, RetryPolicy retryPolicy = null)
        {
            return new SubscriptionClient(_connectionString, topicName, subscriptionName, receiveMode, retryPolicy);
        }
    }
}
