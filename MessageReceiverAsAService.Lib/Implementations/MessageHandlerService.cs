using MessageReceiverAsAService.Lib.Interfaces;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MessageReceiverAsAService.Lib.Implementations
{
    public class MessageHandlerService : IMessageHandlerService
    {
        private readonly ILogger<MessageHandlerService> _logger;
        private readonly ISubscriptionClientFactory _subscriptionClientFactory;
        private readonly IQueueClientMessageHandler _handler;
        private readonly ISessionClientMessageHandler _sessionHandler;

        public MessageHandlerService(ILogger<MessageHandlerService> logger, ISubscriptionClientFactory subscriptionClientFactory, IQueueClientMessageHandler handler, ISessionClientMessageHandler sessionHandler)
        {
            _logger = logger;
            _subscriptionClientFactory = subscriptionClientFactory;
            _handler = handler;
            _sessionHandler = sessionHandler;
        }

        public Task StartListeningAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("StartListeningAsync");

            _handler.Register(stoppingToken);

            // _sessionHandler.Register(stoppingToken);

            _logger.LogInformation("Waiting for messages...");

            // Register the function that processes messages.
            // subscriptionClient.RegisterMessageHandler((message, cancellationToken) => ProcessMessageAsync(subscriptionClient, message, cancellationToken), messageHandlerOptions);

            //var sessionHandlerOptions = new SessionHandlerOptions(ExceptionReceivedHandler)
            //{
            //    MaxConcurrentSessions = 1,
            //    MessageWaitTimeout = TimeSpan.FromSeconds(30),
            //    AutoComplete = false
            //};
            //// Func<IMessageSession, Message, CancellationToken, Task> handler, SessionHandlerOptions sessionHandlerOptions
            //queueClient.RegisterSessionHandler((session, message, cancellationToken) => ProcessSessionMessageAsync(queueClient, session, message, cancellationToken), sessionHandlerOptions);

            return Task.CompletedTask;
        }

        private async Task ProcessSessionMessageAsync(IReceiverClient client, IMessageSession session, Message message, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Type : {ClientType}", client.GetType());

            try
            {
                _logger.LogInformation($"Session       :{session.SessionId}");
                _logger.LogInformation($"SequenceNumber:{message.SystemProperties.SequenceNumber}");
                _logger.LogInformation($"Label         :{message.Label}");
                _logger.LogInformation($"DeliveryCount :{message.SystemProperties.DeliveryCount}");

                string body = Encoding.UTF8.GetString(message.Body);
                if (body == "1")
                {
                    await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken); // simulate delay
                    _logger.LogError("Error: " + DateTime.UtcNow);
                    throw new Exception("!!!");
                }

                //if (message.Label == nameof(PersonMessage))
                //{
                //    var person = _serializer.Deserialize<PersonMessage>(message.Body);
                //    _logger.LogInformation("person = " + JsonConvert.SerializeObject(person));
                //}
                //else
                //{
                //    _logger.LogInformation("other = " + body);
                //}

                _logger.LogInformation("OK : " + body);

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken); // simulate delay

                await session.CompleteAsync(message.SystemProperties.LockToken);
                //await client.CompleteAsync(message.SystemProperties.LockToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while Process Message with MessageId: {MessageId}", message.MessageId);
                if (!session.IsClosedOrClosing)
                {
                    var properties = new Dictionary<string, object>
                    {
                        { "uhuh", DateTime.UtcNow.ToString() }
                    };

                    await session.AbandonAsync(message.SystemProperties.LockToken, properties);
                    //await client.AbandonAsync(message.SystemProperties.LockToken);
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