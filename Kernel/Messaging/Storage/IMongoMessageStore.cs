using Messaging.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Storage
{
    public interface IMongoMessageStore
    {
        Task EnqueueOutboxAsync(OutboxMessage message, CancellationToken cancellationToken);

        Task<IReadOnlyList<OutboxMessage>> GetUnpublishedOutboxAsync(int batchSize, CancellationToken cancellationToken);

        Task MarkOutboxPublishedAsync(OutboxMessage message, CancellationToken cancellationToken);

        Task MarkOutboxFailedAsync(OutboxMessage message, string error, CancellationToken cancellationToken);

        Task<bool> InboxContainsAsync(string consumerGroup, string messageId, CancellationToken cancellationToken);

        Task StoreInboxAsync(InboxMessage message, CancellationToken cancellationToken);

        Task StoreDeadLetterAsync(DeadLetterMessage message, CancellationToken cancellationToken);

        Task<long?> GetOffsetAsync(string consumerGroup, string topic, int partition, CancellationToken cancellationToken);

        Task StoreOffsetAsync(string consumerGroup, string topic, int partition, long offset, CancellationToken cancellationToken);
    }
}
