using Microsoft.Azure.ServiceBus;

namespace MessageReceiverAsAService.Lib.Interfaces
{
    public interface ISubscriptionClientFactory
    {
        ISubscriptionClient Create();
    }
}
