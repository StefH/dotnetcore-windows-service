using WindowsServiceExample.Lib.Implementations;
using WindowsServiceExample.Lib.Interfaces;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace WindowsServiceExample.Lib.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLib([NotNull] this IServiceCollection services)
        {
            services.AddSingleton<IMessageHandlerService, MessageHandlerService>();

            return services;
        }
    }
}