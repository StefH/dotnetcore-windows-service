using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using WindowsServiceExample.Lib.DependencyInjection;

namespace WindowsServiceExample.Net472
{
    /// <summary>
    /// Based on https://github.com/dejanstojanovic/dotnetcore-windows-linux-service/blob/master/Sample.Service.Standard/CommonServiceBase.cs
    /// </summary>
    static class Program
    {
        static void Main(string[] args)
        {
            bool isService = !(Debugger.IsAttached || ((IList)args).Contains("--console"));

            var services = new ServiceCollection();

            // Create configuration builder
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            // Inject configuration
            services.AddSingleton<IConfiguration>(provider => configurationBuilder.Build());

            // Inject Logging
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConsole();
                loggingBuilder.AddDebug();
            });

            // Inject lib
            services.AddLib();

            // Inject concrete implementation of the service
            services.AddSingleton(typeof(ServiceBase), typeof(Service1));

            // Build DI provider
            var serviceProvider = services.BuildServiceProvider();

            var service = serviceProvider.GetService<ServiceBase>();

            if (!isService)
            {
                var svc = (Service1)service;
                svc.StartServiceAsync(args).GetAwaiter().GetResult();
            }
            else
            {
                var servicesToRun = new[] { service };
                ServiceBase.Run(servicesToRun);
            }
        }
    }
}
