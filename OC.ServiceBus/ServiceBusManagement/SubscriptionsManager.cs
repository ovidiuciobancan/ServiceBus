using System;
using System.Threading.Tasks;

using Microsoft.Azure.Management.ServiceBus;
using Microsoft.Azure.Management.ServiceBus.Models;

using OC.ServiceBus.ServiceBusManagement;

namespace OC.ServiceBus
{
    public class SubscriptionsManager : SBEntityManager
    {
        private readonly string _topicName;

        public SubscriptionsManager(ServiceBusManagementClient client, string resourceGroup, string namespaceName, string topicName)
            : base(client, resourceGroup, namespaceName)
        {
            _topicName = topicName;
        }

        public async Task<bool> SubscriptionExists(string subscriptionName)
        {
            var subscription = await Client.Subscriptions.GetAsync(ResourceGroup, Namespace, _topicName, subscriptionName);

            return subscription != null;
        }

        public async Task CreateOrUpdateSubscriptionAsync(string subscriptionName)
        {
            var paramerters = new SBSubscription
            {
                DeadLetteringOnFilterEvaluationExceptions = true,
                DeadLetteringOnMessageExpiration = true,
                MaxDeliveryCount = 10,
                DefaultMessageTimeToLive = new TimeSpan(1, 0, 0)
            };

            await Client.Subscriptions.CreateOrUpdateAsync
            (
                ResourceGroup, 
                Namespace, 
                _topicName, 
                subscriptionName, 
                paramerters
            );
        }

        public async Task DeleteSubscriptionAsync(string subscriptionName)
        {
            await Client.Subscriptions.DeleteAsync(ResourceGroup, Namespace, _topicName, subscriptionName);
        }
    }
}
