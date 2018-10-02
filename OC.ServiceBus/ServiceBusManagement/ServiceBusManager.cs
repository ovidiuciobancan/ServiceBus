using System.Threading.Tasks;

using Microsoft.Azure.Management.ServiceBus;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;

namespace OC.ServiceBus
{
    public class ServiceBusManager
    {
        const string BASE_AUTHORITY = "https://login.microsoftonline.com";
        const string RESOURCE = "https://management.core.windows.net";

        private readonly AuthenticationContext _authenticationContext;
        private readonly ClientCredential _clientCredential;
        private readonly string _subscriptionId;

        public ServiceBusManager(string tenenantId, string clientId, string clientSecret, string  subscriptionId)
        {
            _subscriptionId = subscriptionId;
            _clientCredential = new ClientCredential(clientId, clientSecret);
            _authenticationContext = new AuthenticationContext($"{BASE_AUTHORITY}/{tenenantId}");
        }

        private async Task<ServiceBusManagementClient> GetClientAsync()
        {
            var token = await _authenticationContext.AcquireTokenAsync(RESOURCE, _clientCredential);
            var credential = new TokenCredentials(token.AccessToken);

            return new ServiceBusManagementClient(credential)
            {
                SubscriptionId = _subscriptionId
            };
        }
        
        public async Task<SubscriptionsManager> GetSubscriptions(string resourceGroup, string namespaceName, string topicName)
        {
            var client =  await GetClientAsync();
            return new SubscriptionsManager(client, resourceGroup, namespaceName, topicName);
        }
    }
}
