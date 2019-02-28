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
    internal class SessionClientMessageHandler : BaseMessageHandler, ISessionClientMessageHandler
    {
        private readonly ILogger _logger;
        private readonly ISessionClientFactory _factory;
        private readonly IBinarySerializer _serializer;

        public SessionClientMessageHandler(ILogger<QueueClientMessageHandler> logger, ISessionClientFactory factory, IBinarySerializer serializer) : base(logger)
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

            _logger.LogInformation("Sending session messages...");
            var s = Guid.NewGuid().ToString();
            var m0 = new Message(Encoding.UTF8.GetBytes("0"));
            m0.SessionId = s;
            client.SendAsync(m0).GetAwaiter().GetResult();

            var m1 = new Message(Encoding.UTF8.GetBytes("1"));
            m1.SessionId = s;
            client.SendAsync(m1).GetAwaiter().GetResult();

            var m2 = new Message(Encoding.UTF8.GetBytes("2"));
            m2.SessionId = s;
            client.SendAsync(m2).GetAwaiter().GetResult();

            _logger.LogInformation("Waiting for 3 secs...");
            Task.Delay(TimeSpan.FromSeconds(3), stoppingToken).GetAwaiter().GetResult(); // simulate delay

            var sessionHandlerOptions = new SessionHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentSessions = 1,
                MessageWaitTimeout = TimeSpan.FromSeconds(30),
                AutoComplete = false
            };
            client.RegisterSessionHandler((session, message, cancellationToken) => ProcessMessageAsync(client, session, message, cancellationToken), sessionHandlerOptions);
        }

        private async Task ProcessMessageAsync(IReceiverClient client, IMessageSession session, Message message, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Type : {ClientType}", client.GetType());

            try
            {
                _logger.LogInformation("SystemProperties:{SystemProperties}", JsonConvert.SerializeObject(message.SystemProperties, Formatting.Indented));
                _logger.LogInformation("IMessageSession :{IMessageSession}", JsonConvert.SerializeObject(session, Formatting.Indented));

                string body = Encoding.UTF8.GetString(message.Body);
                if (body == "1")
                {
                    await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken); // simulate delay
                    _logger.LogError("Error: " + DateTime.UtcNow);
                    throw new Exception("!!!");
                }

                if (message.Label == nameof(PersonMessage))
                {
                    var person = _serializer.Deserialize<PersonMessage>(message.Body);
                    _logger.LogInformation("person = " + JsonConvert.SerializeObject(person));
                }
                else
                {
                    _logger.LogInformation("body = {body}", body);
                }

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken); // simulate delay

                await session.CompleteAsync(message.SystemProperties.LockToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while Process Message with MessageId: {MessageId}", message.MessageId);
                if (!session.IsClosedOrClosing)
                {
                    await session.AbandonAsync(message.SystemProperties.LockToken);
                }
            }
        }
    }
}
