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

namespace MessageReceiverAsAService.Lib.Handlers
{
    internal class QueueClientMessageHandler : BaseMessageHandler, IQueueClientMessageHandler
    {
        private readonly ILogger _logger;
        private readonly IQueueClientFactory _factory;
        private readonly IBinarySerializer _serializer;

        public QueueClientMessageHandler(ILogger<QueueClientMessageHandler> logger, IQueueClientFactory factory, IBinarySerializer serializer) : base(logger)
        {
            _logger = logger;
            _factory = factory;
            _serializer = serializer;
        }

        public void Register(CancellationToken stoppingToken)
        {
            IQueueClient client = _factory.Create();

            stoppingToken.Register(async () =>
            {
                _logger.LogInformation("StopListeningAsync");
                if (!client.IsClosedOrClosing)
                {
                    _logger.LogInformation("IQueueClient : close");
                    await client.CloseAsync();
                }
            });

            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                // Maximum number of concurrent calls to the callback ProcessMessagesAsync(), set to 1 for simplicity.
                // Set it according to how many messages the application wants to process in parallel.
                MaxConcurrentCalls = 1,

                // Indicates whether the message pump should automatically complete the messages after returning from user callback.
                // False below indicates the complete operation is handled by the user callback as in ProcessMessagesAsync().
                AutoComplete = false
            };

            client.RegisterMessageHandler((message, cancellationToken) => ProcessMessageAsync(client, message, cancellationToken), messageHandlerOptions);
        }

        private async Task ProcessMessageAsync(IReceiverClient client, Message message, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Type : {ClientType}", client.GetType());

            try
            {
                _logger.LogInformation("SystemProperties:{SystemProperties}", JsonConvert.SerializeObject(message.SystemProperties, Formatting.Indented));

                string body = Encoding.UTF8.GetString(message.Body);

                if (message.Label == nameof(PersonMessage))
                {
                    var person = _serializer.Deserialize<PersonMessage>(message.Body);
                    _logger.LogInformation("person = " + JsonConvert.SerializeObject(person));
                }
                else
                {
                    _logger.LogInformation("body = {body}", body);

                    if (body == "1")
                    {
                        await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken); // simulate delay
                        _logger.LogError("Error: " + DateTime.UtcNow);
                        throw new Exception("!!!");
                    }
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
    }
}
