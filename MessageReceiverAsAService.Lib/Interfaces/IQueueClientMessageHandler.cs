using System.Threading;

namespace MessageReceiverAsAService.Lib.Interfaces
{
    public interface IQueueClientMessageHandler
    {
        void Register(CancellationToken stoppingToken);
    }
}
