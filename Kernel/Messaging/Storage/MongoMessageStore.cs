using Messaging.Configurations;
using Messaging.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Storage
{
    public class MongoMessageStore : IMongoMessageStore
    {
        private readonly IMongoCollection<OutboxMessage> _outbox;
        private readonly IMongoCollection<InboxMessage> _inbox;
        private readonly IMongoCollection<DeadLetterMessage> _deadLetters;
        private readonly IMongoCollection<ConsumerOffset> _offsets;

        public MongoMessageStore(IMongoClient client, IOptions<MongoOptions> options)
        {
            var mongoOptions = options.Value;
            var database = client.GetDatabase(mongoOptions.DatabaseName);
            _outbox = database.GetCollection<OutboxMessage>(mongoOptions.OutboxCollectionName);
            _inbox = database.GetCollection<InboxMessage>(mongoOptions.InboxCollectionName);
            _deadLetters = database.GetCollection<DeadLetterMessage>(mongoOptions.DeadLetterCollectionName);
            _offsets = database.GetCollection<ConsumerOffset>(mongoOptions.OffsetsCollectionName);

            CreateIndexes();
        }

        private void CreateIndexes()
        {
            _outbox.Indexes.CreateOne(new CreateIndexModel<OutboxMessage>(Builders<OutboxMessage>.IndexKeys.Ascending(x => x.PublishedAt)));
            _inbox.Indexes.CreateOne(new CreateIndexModel<InboxMessage>(Builders<InboxMessage>.IndexKeys.Ascending(x => x.ConsumerGroup).Ascending(x => x.MessageId), new CreateIndexOptions { Unique = true }));
            _deadLetters.Indexes.CreateOne(new CreateIndexModel<DeadLetterMessage>(Builders<DeadLetterMessage>.IndexKeys.Ascending(x => x.Topic).Descending(x => x.FailedAt)));
            _offsets.Indexes.CreateOne(new CreateIndexModel<ConsumerOffset>(Builders<ConsumerOffset>.IndexKeys.Ascending(x => x.ConsumerGroup).Ascending(x => x.Topic).Ascending(x => x.Partition), new CreateIndexOptions { Unique = true }));
        }

        public async Task EnqueueOutboxAsync(OutboxMessage message, CancellationToken cancellationToken)
        {
            await _outbox.InsertOneAsync(message, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<IReadOnlyList<OutboxMessage>> GetUnpublishedOutboxAsync(int batchSize, CancellationToken cancellationToken)
        {
            var filter = Builders<OutboxMessage>.Filter.Eq(x => x.PublishedAt, null);
            var cursor = await _outbox.Find(filter).SortBy(x => x.CreatedAt).Limit(batchSize).ToListAsync(cancellationToken).ConfigureAwait(false);
            return cursor;
        }

        public Task MarkOutboxPublishedAsync(OutboxMessage message, CancellationToken cancellationToken)
        {
            var update = Builders<OutboxMessage>.Update
                .Set(x => x.PublishedAt, DateTimeOffset.UtcNow)
                .Set(x => x.AttemptCount, message.AttemptCount + 1)
                .Set(x => x.LastError, null);
            return _outbox.UpdateOneAsync(x => x.Id == message.Id, update, cancellationToken: cancellationToken);
        }

        public Task MarkOutboxFailedAsync(OutboxMessage message, string error, CancellationToken cancellationToken)
        {
            var update = Builders<OutboxMessage>.Update
                .Set(x => x.AttemptCount, message.AttemptCount + 1)
                .Set(x => x.LastError, error);
            return _outbox.UpdateOneAsync(x => x.Id == message.Id, update, cancellationToken: cancellationToken);
        }

        public async Task<bool> InboxContainsAsync(string consumerGroup, string messageId, CancellationToken cancellationToken)
        {
            var filter = Builders<InboxMessage>.Filter.Eq(x => x.ConsumerGroup, consumerGroup) & Builders<InboxMessage>.Filter.Eq(x => x.MessageId, messageId);
            var existing = await _inbox.Find(filter).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
            return existing != null;
        }

        public Task StoreInboxAsync(InboxMessage message, CancellationToken cancellationToken)
        {
            return _inbox.InsertOneAsync(message, cancellationToken: cancellationToken);
        }

        public Task StoreDeadLetterAsync(DeadLetterMessage message, CancellationToken cancellationToken)
        {
            return _deadLetters.InsertOneAsync(message, cancellationToken: cancellationToken);
        }

        public async Task<long?> GetOffsetAsync(string consumerGroup, string topic, int partition, CancellationToken cancellationToken)
        {
            var filter = BuildOffsetFilter(consumerGroup, topic, partition);
            var offset = await _offsets.Find(filter).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
            return offset?.Offset;
        }

        public Task StoreOffsetAsync(string consumerGroup, string topic, int partition, long offset, CancellationToken cancellationToken)
        {
            var filter = BuildOffsetFilter(consumerGroup, topic, partition);
            var update = Builders<ConsumerOffset>.Update
                .SetOnInsert(x => x.ConsumerGroup, consumerGroup)
                .SetOnInsert(x => x.Topic, topic)
                .SetOnInsert(x => x.Partition, partition)
                .Set(x => x.Offset, offset);

            return _offsets.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true }, cancellationToken);
        }

        private static FilterDefinition<ConsumerOffset> BuildOffsetFilter(string consumerGroup, string topic, int partition)
        {
            return Builders<ConsumerOffset>.Filter.Eq(x => x.ConsumerGroup, consumerGroup)
                & Builders<ConsumerOffset>.Filter.Eq(x => x.Topic, topic)
                & Builders<ConsumerOffset>.Filter.Eq(x => x.Partition, partition);
        }
    }
}
