using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Processing
{
    public class KafkaConsumerRegistration
    {
        public required string Topic { get; init; }

        public required string GroupId { get; init; }

        public required Type MessageType { get; init; }

        public required Type HandlerType { get; init; }
    }
}
