using Messaging.Entities;
using Confluent.Kafka;
using Messaging.Configurations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Messaging.Storage;
using Messaging.Serializations;

namespace Messaging.Processing
{
    public class KafkaMessagePublisher : IMessagePublisher
    {
        private readonly IMongoMessageStore _store;
        private readonly IMessageSerializer _serializer;

        public KafkaMessagePublisher(IMongoMessageStore store, IMessageSerializer serializer)
        {
            _store = store;
            _serializer = serializer;
        }

        public async Task PublishAsync<T>(string topic, string? key, T message, IDictionary<string, string?>? headers = null, CancellationToken cancellationToken = default)
        {
            headers ??= new Dictionary<string, string?>();
            if (!headers.ContainsKey("message-id"))
            {
                headers["message-id"] = Guid.NewGuid().ToString();
            }
            var payload = _serializer.Serialize(message, headers);

            var outboxMessage = new OutboxMessage
            {
                Topic = topic,
                Key = key,
                Payload = payload,
                Headers = headers
            };

            await _store.EnqueueOutboxAsync(outboxMessage, cancellationToken).ConfigureAwait(false);
        }
    }
}
