README v1.0 / 01 October 2018

# Project name

**OC.ServiceBus**

## Introduction

OC.ServiceBus is a .NET Core class library used to create and manage Azure Service Bus Entities (Queues, Topics, Subscriptions) .    

## Usage

Here you can find some examples for creating a sender and  a receiver.

##### Message

```
    class MyMessage 
    {
        public string Title { get; set; }
        public string Body { get; set; }
    }
```

##### Sender

```
    //the connection string for azure service bus resource
    string connectionString = "Endpoint=sb://myazureservicebus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=<sharedAccessKey>";

    //the logger that is configured for the application
    ILogger logger = _logger;

    //factory that creates any type of entities: queue, topic,... 
    var entitiesFactory = new SBEntitiesFactory(connectionString);

    //get topic by name that is already created in azure 
    var topic = entitiesFactory.GetTopic("my-topic") 

    //create the sender that is gonna use this topic
    var sender = new ServiceBusSender(topic, logger);

    //create message
    var message = new MyMessage 
    {
        Title = "MessageTitle", 
        Body = "MessageBody"
    };

    //send message to topic async
    async sender.SendMessageAsync(message);

```

* **connectionString** - more info [here](https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-dotnet-how-to-use-topics-subscriptions#obtain-the-management-credentials) 
* **logger** - more info [here](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-2.1)

#### Receiver

```
    //the connection string for azure service bus resource
    string connectionString = "Endpoint=sb://myazureservicebus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=<sharedAccessKey>";

    //the logger that is configured for the application
    ILogger logger = _logger;

    //factory that creates any type of entities: queue, topic,... 
    var entitiesFactory = new SBEntitiesFactory(connectionString);

    //get subscription by name and topic name that is already created in azure 
    var subscription = entitiesFactory.GetSubscription("my-topic", "my-topic-subscription"); 

    //create receiver that listen to a subscription
    var receiver = new ServiceBusReceiver(subscription, logger);

    //create an object that will processor the received message
    var messageProcessor = new MyMessageProcessor();

    //subscribe all the processors
    //in this case we have only one
    receiver.SubscribeProcessors(messageProcessor);

    //Register processors
    receiver.Register();
```

##### MessageProcessor

```
    class MyMessageProcessor : IMessageProcessor
    {
        private readonly IServiceProvider _serviceProvider;

        public MessageProcessor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task ProcessMessageAsync(string textMessage)
        {
            //deserialize the text message
            var message = JsonConvert.DeserializeObject<MyMessage>(textMessage);

            /*
                this is useful when you need other objects in custom scope 
                for example, let say that you have an ASP.Core project and you use EF Core to persist data
                you need to get the EF DbContext in a custom scope
            */
            //create custom scope 
            using (var scope = _serviceProvider.CreateScope())
            {
                //get context in a custom scope 
                var context = scope.ServiceProvider.GetService<AppDbContext>();

                //Do stuff 

                //Persist data
                context.MyMessages.Add(message); 

                //Save changes
                await context.SaveChangesAsync();
            }
        }
    }
```

## Contributing

If you're willing to contribute, 
first you need to know what design patterns are used
and what modules are opened for extension.

###### SBClientsFactory

Both __Sender__ and __Receiver__ classes use _Strategy Design Pattern_,
the behavior of those classes it's passed in the constructor.
The only constraint is that the behavior class must implement 
_Microsoft.Azure.ServiceBus.Core.ISenderClient_ or 
_Microsoft.Azure.ServiceBus.Core.IReceiverClient_. 

For example if you want to create a receiver with a Relay behavior,  
you need implement GetRelay method in SBEntitiesFactory that returns an object that implements _IReceiverClient_.   

###### ServiceBusManager

This module is used to create and manage service bus entities programmatically.
Right now, the module provide implementation only for Subscriptions, 
but can be extended for Queues, Topics, etc.

For example if you want to include management for _Topics_, you need to create a class that implements SBEntity.
After that you need to create a method in _ServiceBusManager_, named GetTopics   

## Installation

### Requirements

To use this project you need an Azure account.
[Here](https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-create-namespace-portal) you can find more information.

### Installation

This class library can be packed in a nuget package, 
and can be retrieved directly from the nuget server 

### Configuration

No configuration is needed for this project.

## Credits

Ovidiu Ciobancan

## Contact

ovidiu.ciobancan.dev@gmail.com
