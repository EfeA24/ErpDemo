using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Processing
{
    public sealed class MessageContext
    {
        public MessageContext(string topic, int partition, long offset, string? key, IDictionary<string, string?> headers)
        {
            Topic = topic;
            Partition = partition;
            Offset = offset;
            Key = key;
            Headers = headers;
        }

        public string Topic { get; }

        public int Partition { get; }

        public long Offset { get; }

        public string? Key { get; }

        public IDictionary<string, string?> Headers { get; }

        public string GetMessageId() => Headers.TryGetValue("message-id", out var id) && !string.IsNullOrWhiteSpace(id)
            ? id!
            : $"{Topic}:{Partition}:{Offset}";

        public string ConsumerGroup { get; internal set; } = string.Empty;

        public DateTimeOffset ConsumedAt { get; internal set; } = DateTimeOffset.UtcNow;
    }
}
