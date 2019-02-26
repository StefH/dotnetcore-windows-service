using MessageReceiverAsAService.Lib.Options;
using MessageReceiverAsAService.ServiceBase;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventLog;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MessageReceiverAsAService
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            bool isService = !(Debugger.IsAttached || args.Contains("--console"));

            var builder = new HostBuilder()
                .ConfigureHostConfiguration(config =>
                {
                    config.AddEnvironmentVariables();
                })
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: false);
                    config.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true);
                    config.AddEnvironmentVariables();
                })
                .ConfigureLogging((hostContext, loggingBuilder) =>
                {
                    loggingBuilder.AddDebug();
                    loggingBuilder.AddConsole();
                    var settings = new EventLogSettings
                    {
                        SourceName = "MessageReceiverAsAService",
                        Filter = (source, level) =>
                        {
                            return level >= LogLevel.Information;
                        }
                    };

                    loggingBuilder.AddEventLog(settings);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddOptions();
                    services.AddSingleton<IConfigureOptions<ServiceBusSubscriptionOptions>>(serviceProvider =>
                    {
                        var section = serviceProvider.GetRequiredService<IConfiguration>().GetSection("ServiceBusSubscriptionOptions");
                        return new NamedConfigureFromConfigurationOptions<ServiceBusSubscriptionOptions>(string.Empty, section);
                    });

                    services.AddMessageReceiver();

                    services.AddHostedService<MessageReceiverService>();
                });

            RunAsync(builder, isService).GetAwaiter().GetResult();
        }

        private static Task RunAsync(IHostBuilder builder, bool isService)
        {
            EventLog l = new EventLog("MessageReceiverAsAService-Program", Environment.MachineName, nameof(Program));
            l.WriteEntry("RunAsync", EventLogEntryType.Information, 2000);

            return isService ? builder.RunAsServiceAsync() : builder.RunConsoleAsync();
        }
    }
}