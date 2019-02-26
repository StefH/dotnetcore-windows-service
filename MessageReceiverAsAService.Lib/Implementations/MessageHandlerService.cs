using MessageReceiverAsAService.Lib.Interfaces;
using MessageReceiverAsAService.Lib.Models;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MessageReceiverAsAService.Lib.Implementations
{
    public class MessageHandlerService : IMessageHandlerService
    {
        private readonly ILogger<MessageHandlerService> _logger;
        private readonly ISubscriptionClientFactory _subscriptionClientFactory;
        private readonly IQueueClientFactory _queueClientFactory;
        private readonly IBinarySerializer _serializer;

        public MessageHandlerService(ILogger<MessageHandlerService> logger, ISubscriptionClientFactory subscriptionClientFactory, IQueueClientFactory queueClientFactory, IBinarySerializer serializer)
        {
            _logger = logger;
            _subscriptionClientFactory = subscriptionClientFactory;
            _queueClientFactory = queueClientFactory;
            _serializer = serializer;
        }

        public Task StartListeningAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("StartListeningAsync");

            IQueueClient queueClient = _queueClientFactory.Create();

            ISubscriptionClient subscriptionClient = _subscriptionClientFactory.Create();

            stoppingToken.Register(async () =>
            {
                _logger.LogInformation("StopListeningAsync");
                if (!subscriptionClient.IsClosedOrClosing)
                {
                    _logger.LogInformation("ISubscriptionClient : close");
                    await subscriptionClient.CloseAsync();
                }

                if (!queueClient.IsClosedOrClosing)
                {
                    _logger.LogInformation("IQueueClient : close");
                    await queueClient.CloseAsync();
                }
            });

            // Configure the message handler options in terms of exception handling, number of concurrent messages to deliver, etc.
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                // Maximum number of concurrent calls to the callback ProcessMessagesAsync(), set to 1 for simplicity.
                // Set it according to how many messages the application wants to process in parallel.
                MaxConcurrentCalls = 2,

                // Indicates whether the message pump should automatically complete the messages after returning from user callback.
                // False below indicates the complete operation is handled by the user callback as in ProcessMessagesAsync().
                AutoComplete = false
            };

            _logger.LogInformation("Waiting for messages...");

            // Register the function that processes messages.
            subscriptionClient.RegisterMessageHandler((message, cancellationToken) => ProcessMessageAsync(subscriptionClient, message, cancellationToken), messageHandlerOptions);
            queueClient.RegisterMessageHandler((message, cancellationToken) => ProcessMessageAsync(queueClient, message, cancellationToken), messageHandlerOptions);

            return Task.CompletedTask;
        }

        private async Task ProcessMessageAsync(IReceiverClient client, Message message, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Type : {ClientType}", client.GetType());

            try
            {
                _logger.LogInformation($"SequenceNumber:{message.SystemProperties.SequenceNumber} Label:{message.Label}");

                if (message.Label == nameof(PersonMessage))
                {
                    var person = _serializer.Deserialize<PersonMessage>(message.Body);
                    _logger.LogInformation("person = " + JsonConvert.SerializeObject(person));
                }
                else
                {
                    _logger.LogInformation("other = " + Encoding.UTF8.GetString(message.Body));
                }

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken); // simulate delay

                await client.CompleteAsync(message.SystemProperties.LockToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while Process Message with MessageId: {MessageId}", message.MessageId);
                if (!client.IsClosedOrClosing)
                {
                    await client.AbandonAsync(message.SystemProperties.LockToken);
                }
            }
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            _logger.LogError(exceptionReceivedEventArgs.Exception, $"Endpoint: {context.Endpoint} | Entity Path: {context.EntityPath} | Executing Action: {context.Action}");

            return Task.CompletedTask;
        }
    }
}