using Microsoft.Azure.ServiceBus;

namespace MessageReceiverAsAService.Lib.Interfaces
{
    public interface ISessionClientFactory
    {
        IQueueClient Create();
    }
}
