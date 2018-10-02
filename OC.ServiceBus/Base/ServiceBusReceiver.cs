using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Extensions.Logging;

using OC.ServiceBus.Interfaces;


namespace OC.ServiceBus
{
    public class ServiceBusReceiver
    {
        private ILogger _logger;
        private IReceiverClient _client;
        private Func<IReceiverClient, Task> _onClose;
        private readonly List<IMessageProcessor> _messageProcessors;

        public ServiceBusReceiver(IReceiverClient client, ILogger logger)
        {
            _client = client;
            _logger = logger;
            _messageProcessors = new List<IMessageProcessor>();
        }


        public ServiceBusReceiver SubscribeProcessors(params IMessageProcessor[] processors)
        {
            _messageProcessors.AddRange(processors);
            return this;
        }

        public ServiceBusReceiver UnsubscribeProcessors(params IMessageProcessor[] processors)
        {
            processors.ToList().ForEach(p => _messageProcessors.Remove(p));
            return this;
        }

        public ServiceBusReceiver UnsubscribeAll()
        {
            _messageProcessors.Clear();
            return this;
        }

        public void OnClose(Func<IReceiverClient, Task> action)
        {
            //Register the function that will be called before the client is closed
            _onClose = action;
        }

        public async Task CloseAsync()
        {
            await _onClose?.Invoke(_client);
            await _client.CloseAsync();
        }

        public void Register()
        {
            // Configure the MessageHandler Options in terms of exception handling, number of concurrent messages to deliver etc.
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                // Maximum number of Concurrent calls to the callback `ProcessMessagesAsync`, set to 1 for simplicity.
                // Set it according to how many messages the application wants to process in parallel.
                MaxConcurrentCalls = 10,

                // Indicates whether MessagePump should automatically complete the messages after returning from User Callback.
                // False below indicates the Complete will be handled by the User Callback as in `ProcessMessagesAsync` below.
                AutoComplete = false,
                
            };

            // Register the function that will process messages
            _client.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
        }
        private async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {

            _logger.LogInformation($"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber}");
            
            // Process the message
            try
            {
                var tasks = _messageProcessors.Select(p => p.ProcessMessageAsync(Encoding.UTF8.GetString(message.Body)));

                await Task.WhenAll(tasks);

                // Complete the message so that it is not received again.
                // This can be done only if the queueClient is created in ReceiveMode.PeekLock mode (which is default).
                await _client.CompleteAsync(message.SystemProperties.LockToken);
            }
            catch (Exception ex)
            {
                await _client.AbandonAsync(message.SystemProperties.LockToken);
                _logger.LogError(ex, $"Cannot process message [Id: {message.MessageId}]");
            }


            // Note: Use the cancellationToken passed as necessary to determine if the queueClient has already been closed.
            // If queueClient has already been Closed, you may chose to not call CompleteAsync() or AbandonAsync() etc. calls 
            // to avoid unnecessary exceptions.
        }
        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            var exception = exceptionReceivedEventArgs.Exception;

            _logger.LogError(exception, $"Message handler encountered an exception {exception.Message}");
            _logger.LogInformation(exceptionReceivedEventArgs.Exception, $@"
                Exception context for troubleshooting:
                    - Endpoint: { context.Endpoint}
                    - Entity Path: { context.EntityPath}
                    - Executing Action: { context.Action}
            ");

            return Task.CompletedTask;
        }
    }
}
