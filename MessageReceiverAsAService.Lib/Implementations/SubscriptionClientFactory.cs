using MessageReceiverAsAService.Lib.Interfaces;
using MessageReceiverAsAService.Lib.Options;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Options;

namespace MessageReceiverAsAService.Lib.Implementations
{
    public class SubscriptionClientFactory : ISubscriptionClientFactory
    {
        private readonly ServiceBusSubscriptionOptions _options;

        public SubscriptionClientFactory(IOptions<ServiceBusSubscriptionOptions> options)
        {
            _options = options.Value;
        }

        public ISubscriptionClient Create()
        {
            return new SubscriptionClient(_options.ConnectionString, _options.TopicPath, _options.SubscriptionName);
        }
    }
}