using System;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace OC.ServiceBus
{
    public class ServiceBusSender
    {
        private ILogger logger;
        private ISenderClient client;
        public ServiceBusSender(ISenderClient client, ILogger logger) 
        {
            this.client = client;
            this.logger = logger;
        }

        public async Task SendMessageAsync(object message, Action<Message> action = null)
        {
            try
            {
                var serializedMessage = JsonConvert.SerializeObject(message);

                var msg = new Message(Encoding.UTF8.GetBytes(serializedMessage));

                action?.Invoke(msg);

                await client.SendAsync(msg);
            }
            catch (Exception e)
            {
                logger.LogError(e, $"{DateTime.Now} :: Exception: {e.Message}");
            }
        }
    }
}
