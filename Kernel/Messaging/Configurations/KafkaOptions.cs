using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Configurations
{
    public class KafkaOptions
    {
        public const string SectionName = "Messaging:Kafka";

        public string BootstrapServers { get; set; } = string.Empty;

        public string? ClientId { get; set; }

        public int OutboxBatchSize { get; set; } = 100;

        public TimeSpan OutboxDispatchInterval { get; set; } = TimeSpan.FromSeconds(5);

        public int ConsumerPollIntervalMs { get; set; } = 1000;

        public TimeSpan ConsumerCommitInterval { get; set; } = TimeSpan.FromSeconds(5);
    }
}
