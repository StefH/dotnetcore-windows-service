using System.Threading;

namespace MessageReceiverAsAService.Lib.Interfaces
{
    public interface ISessionClientMessageHandler
    {
        void Register(CancellationToken stoppingToken);
    }
}
