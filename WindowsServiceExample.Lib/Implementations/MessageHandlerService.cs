using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using WindowsServiceExample.Lib.Interfaces;

namespace WindowsServiceExample.Lib.Implementations
{
    public class MessageHandlerService : IMessageHandlerService
    {
        private readonly ILogger<MessageHandlerService> _logger;

        public MessageHandlerService(ILogger<MessageHandlerService> logger)
        {
            _logger = logger;
        }

        public async Task StartListeningAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("StartListeningAsync");

            stoppingToken.Register(() => _logger.LogInformation("StopListeningAsync"));

            // Do some work ...
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Waiting...");
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }
    }
}