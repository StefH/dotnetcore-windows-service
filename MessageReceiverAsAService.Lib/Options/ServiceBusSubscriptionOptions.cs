using System.ComponentModel.DataAnnotations;

namespace MessageReceiverAsAService.Lib.Options
{
    public class ServiceBusSubscriptionOptions
    {
        // [RegularExpression("^.*$")]
        [Required]
        public string ConnectionString { get; set; }

        [Required]
        public string TopicPath { get; set; }

        [Required]
        public string SubscriptionName { get; set; }

        [Required]
        public string QueueName { get; set; }

        [Required]
        public string SessionQueueName { get; set; }
    }
}
