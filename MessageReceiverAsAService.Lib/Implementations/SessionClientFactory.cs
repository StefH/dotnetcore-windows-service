using MessageReceiverAsAService.Lib.Interfaces;
using MessageReceiverAsAService.Lib.Options;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Options;

namespace MessageReceiverAsAService.Lib.Implementations
{
    public class SessionClientFactory : ISessionClientFactory
    {
        private readonly ServiceBusSubscriptionOptions _options;

        private readonly ServiceBusConnection _connection;

        public SessionClientFactory(IOptions<ServiceBusSubscriptionOptions> options)
        {
            _options = options.Value;

            _connection = new ServiceBusConnection(_options.ConnectionString);
        }

        public IQueueClient Create()
        {
            return new QueueClient(_connection, _options.SessionQueueName, ReceiveMode.PeekLock, RetryPolicy.Default);
        }
    }
}