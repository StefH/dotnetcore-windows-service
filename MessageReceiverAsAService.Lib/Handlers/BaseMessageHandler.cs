using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;

namespace MessageReceiverAsAService.Lib.Handlers
{
    internal abstract class BaseMessageHandler
    {
        private readonly ILogger _logger;

        protected BaseMessageHandler(ILogger<BaseMessageHandler> logger)
        {
            _logger = logger;
        }

        protected Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            _logger.LogError(exceptionReceivedEventArgs.Exception, $"Endpoint: {context.Endpoint} | Entity Path: {context.EntityPath} | Executing Action: {context.Action}");

            return Task.CompletedTask;
        }
    }
}
