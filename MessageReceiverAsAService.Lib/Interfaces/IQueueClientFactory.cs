using Microsoft.Azure.ServiceBus;

namespace MessageReceiverAsAService.Lib.Interfaces
{
    public interface IQueueClientFactory
    {
        IQueueClient Create();
    }
}
