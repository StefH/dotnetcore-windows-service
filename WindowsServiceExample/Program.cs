using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventLog;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WindowsServiceExample.Lib.DependencyInjection;
using WindowsServiceExample.ServiceBase;

namespace WindowsServiceExample
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
                        SourceName = "WindowsServiceExample",
                        Filter = (source, level) => level >= LogLevel.Information
                    };

                    loggingBuilder.AddEventLog(settings);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddLib();

                    services.AddHostedService<MessageReceiverService>();
                });

            RunAsync(builder, isService).GetAwaiter().GetResult();
        }

        private static Task RunAsync(IHostBuilder builder, bool isService)
        {
            return isService ? builder.RunAsServiceAsync() : builder.RunConsoleAsync();
        }
    }
}