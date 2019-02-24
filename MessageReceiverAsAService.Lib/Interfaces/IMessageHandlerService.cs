using System.Threading;
using System.Threading.Tasks;

namespace MessageReceiverAsAService.Lib.Interfaces
{
    public interface IMessageHandlerService
    {
        /// <summary>
        /// This method is starts listening for messages.
        /// </summary>
        /// <param name="stoppingToken">Triggered when the caller indicates that listening should be stopped.</param>
        /// <returns>A <see cref="Task"/> that represents the long running operations.</returns>
        Task StartListeningAsync(CancellationToken stoppingToken);
    }
}