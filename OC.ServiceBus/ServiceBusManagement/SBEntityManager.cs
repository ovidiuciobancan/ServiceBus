using System;
using Microsoft.Azure.Management.ServiceBus;

namespace OC.ServiceBus.ServiceBusManagement
{
    public abstract class SBEntityManager
    {
        protected string ResourceGroup { get;  }
        protected string Namespace { get; }
        protected ServiceBusManagementClient Client { get; }

        public SBEntityManager(ServiceBusManagementClient client, string resourceGroup, string namespaceName)
        {
            if (String.IsNullOrWhiteSpace(resourceGroup)) throw new ArgumentNullException(nameof(resourceGroup));
            if (String.IsNullOrWhiteSpace(namespaceName)) throw new ArgumentNullException(nameof(namespaceName));

            Client = client ?? throw new ArgumentNullException(nameof(client));

            ResourceGroup = resourceGroup;
            Namespace = namespaceName;
        }
    }
}
