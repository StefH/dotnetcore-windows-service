﻿using MessageReceiverAsAService.Lib.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace MessageReceiverAsAService
{
    [UsedImplicitly]
    public class MessageReceiverService : BackgroundService
    {
        private readonly ILogger<MessageReceiverService> _logger;
        private readonly IMessageHandlerService _service;

        public MessageReceiverService(ILogger<MessageReceiverService> logger, IMessageHandlerService service)
        {
            _logger = logger;
            _service = service;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ExecuteAsync");

            _logger.LogInformation("Waiting 5 seconds before starting to listen");
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

            await _service.StartListeningAsync(stoppingToken);
        }
    }
}