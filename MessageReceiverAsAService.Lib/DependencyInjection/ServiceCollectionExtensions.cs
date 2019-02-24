using JetBrains.Annotations;
using MessageReceiverAsAService.Lib.Implementations;
using MessageReceiverAsAService.Lib.Interfaces;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace MessageReceiverAsAService.Lib.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMessageReceiver([NotNull] this IServiceCollection services)
        {
            services.AddSingleton<IBinarySerializer, BinarySerializer>();
            services.AddSingleton<ISubscriptionClientFactory, SubscriptionClientFactory>();
            services.AddSingleton<IMessageHandlerService, MessageHandlerService>();

            return services;
        }
    }
}