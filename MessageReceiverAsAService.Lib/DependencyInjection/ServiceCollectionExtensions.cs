using JetBrains.Annotations;
using MessageReceiverAsAService.Lib.Handlers;
using MessageReceiverAsAService.Lib.Implementations;
using MessageReceiverAsAService.Lib.Interfaces;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMessageReceiver([NotNull] this IServiceCollection services)
        {
            services.AddSingleton<IBinarySerializer, BinarySerializer>();
            services.AddSingleton<ISubscriptionClientFactory, SubscriptionClientFactory>();
            services.AddSingleton<IQueueClientFactory, QueueClientFactory>();
            services.AddSingleton<ISessionClientFactory, SessionClientFactory>();
            services.AddSingleton<IMessageHandlerService, MessageHandlerService>();
            services.AddSingleton<IQueueClientMessageHandler, QueueClientMessageHandler>();
            services.AddSingleton<ISessionClientMessageHandler, SessionClientMessageHandler>();

            return services;
        }
    }
}