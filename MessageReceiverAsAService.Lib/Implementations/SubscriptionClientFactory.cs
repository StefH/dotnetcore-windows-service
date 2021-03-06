﻿using MessageReceiverAsAService.Lib.Interfaces;
using MessageReceiverAsAService.Lib.Options;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Options;

namespace MessageReceiverAsAService.Lib.Implementations
{
    public class SubscriptionClientFactory : ISubscriptionClientFactory
    {
        private readonly ServiceBusSubscriptionOptions _options;

        private readonly ServiceBusConnection _connection;

        public SubscriptionClientFactory(IOptions<ServiceBusSubscriptionOptions> options)
        {
            _options = options.Value;

            _connection = new ServiceBusConnection(_options.ConnectionString);
        }

        public ISubscriptionClient Create()
        {
            return new SubscriptionClient(_connection, _options.TopicPath, _options.SubscriptionName, ReceiveMode.PeekLock, RetryPolicy.Default);
        }
    }
}