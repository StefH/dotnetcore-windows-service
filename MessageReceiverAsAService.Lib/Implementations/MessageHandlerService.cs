using MessageReceiverAsAService.Lib.Interfaces;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MessageReceiverAsAService.Lib.Models;

namespace MessageReceiverAsAService.Lib.Implementations
{
    public class MessageHandlerService : IMessageHandlerService
    {
        private readonly ILogger<MessageHandlerService> _logger;
        private readonly ISubscriptionClientFactory _factory;
        private readonly IBinarySerializer _serializer;

        public MessageHandlerService(ILogger<MessageHandlerService> logger, ISubscriptionClientFactory factory, IBinarySerializer serializer)
        {
            _logger = logger;
            _factory = factory;
            _serializer = serializer;
        }

        public Task StartListeningAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("StartListeningAsync");

            ISubscriptionClient client = _factory.Create();

            stoppingToken.Register(async () =>
            {
                _logger.LogInformation("StopListeningAsync");
                if (!client.IsClosedOrClosing)
                {
                    await client.CloseAsync();
                }
            });

            // Configure the message handler options in terms of exception handling, number of concurrent messages to deliver, etc.
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                // Maximum number of concurrent calls to the callback ProcessMessagesAsync(), set to 1 for simplicity.
                // Set it according to how many messages the application wants to process in parallel.
                MaxConcurrentCalls = 1,

                // Indicates whether the message pump should automatically complete the messages after returning from user callback.
                // False below indicates the complete operation is handled by the user callback as in ProcessMessagesAsync().
                AutoComplete = false
            };

            _logger.LogInformation("Waiting for messages...");

            // Register the function that processes messages.
            client.RegisterMessageHandler((message, cancellationToken) => ProcessMessagesAsync(client, message, cancellationToken), messageHandlerOptions);

            return Task.CompletedTask;
        }

        private async Task ProcessMessagesAsync(ISubscriptionClient client, Message message, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"NORM: SequenceNumber:{message.SystemProperties.SequenceNumber} Label:{message.Label}");

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
            catch
            {
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