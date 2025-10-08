using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Messaging.Configurations;
using Messaging.Entities;
using Messaging.Serializations;
using Messaging.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Messaging.Processing
{
    public class KafkaConsumerHostedService : BackgroundService
    {
        private readonly IReadOnlyCollection<KafkaConsumerRegistration> _registrations;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMessageSerializer _serializer;
        private readonly IMongoMessageStore _store;
        private readonly KafkaOptions _kafkaOptions;
        private readonly ILoggerFactory _loggerFactory;

        public KafkaConsumerHostedService(
            IEnumerable<KafkaConsumerRegistration> registrations,
            IServiceScopeFactory scopeFactory,
            IMessageSerializer serializer,
            IMongoMessageStore store,
            IOptions<KafkaOptions> kafkaOptions,
            ILoggerFactory loggerFactory)
        {
            _registrations = registrations.ToArray();
            _scopeFactory = scopeFactory;
            _serializer = serializer;
            _store = store;
            _kafkaOptions = kafkaOptions.Value;
            _loggerFactory = loggerFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_registrations.Count == 0)
            {
                return;
            }

            var tasks = _registrations.Select(registration => RunConsumerAsync(registration, stoppingToken)).ToArray();
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        private async Task RunConsumerAsync(KafkaConsumerRegistration registration, CancellationToken cancellationToken)
        {
            var logger = _loggerFactory.CreateLogger($"KafkaConsumer[{registration.Topic}/{registration.GroupId}]");

            var config = new ConsumerConfig
            {
                BootstrapServers = _kafkaOptions.BootstrapServers,
                GroupId = registration.GroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false,
                EnablePartitionEof = false,
                ClientId = _kafkaOptions.ClientId
            };

            using var consumer = new ConsumerBuilder<string?, string>(config)
                .SetErrorHandler((_, error) => logger.LogError("Kafka consumer error: {Error}", error))
                .SetPartitionsAssignedHandler((c, partitions) =>
                {
                    var assignments = new List<TopicPartitionOffset>();
                    foreach (var partition in partitions)
                    {
                        var storedOffset = _store.GetOffsetAsync(registration.GroupId, partition.Topic, partition.Partition.Value, CancellationToken.None)
                            .GetAwaiter().GetResult();

                        assignments.Add(storedOffset.HasValue
                            ? new TopicPartitionOffset(partition, new Offset(storedOffset.Value + 1))
                            : new TopicPartitionOffset(partition, Offset.Unset));
                    }

                    c.Assign(assignments);
                })
                .SetPartitionsRevokedHandler((c, _) => c.Unassign())
                .Build();

            consumer.Subscribe(registration.Topic);

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    ConsumeResult<string?, string>? result = null;
                    try
                    {
                        result = consumer.Consume(cancellationToken);
                    }
                    catch (ConsumeException ex)
                    {
                        logger.LogError(ex, "Kafka consume exception");
                        continue;
                    }

                    if (result is null)
                    {
                        continue;
                    }

                    var headers = ConvertHeaders(result.Message.Headers);
                    var context = new MessageContext(result.Topic, result.Partition.Value, result.Offset.Value, result.Message.Key, headers)
                    {
                        ConsumerGroup = registration.GroupId,
                        ConsumedAt = DateTimeOffset.UtcNow
                    };

                    var messageId = context.GetMessageId();

                    if (await _store.InboxContainsAsync(registration.GroupId, messageId, cancellationToken).ConfigureAwait(false))
                    {
                        TryCommit(consumer, result, logger);
                        continue;
                    }

                    object? payload = null;
                    try
                    {
                        payload = _serializer.Deserialize(result.Message.Value, headers, registration.MessageType);
                        if (payload is null)
                        {
                            throw new InvalidOperationException($"Failed to deserialize message for topic {registration.Topic}");
                        }

                        using var scope = _scopeFactory.CreateScope();
                        var handler = scope.ServiceProvider.GetRequiredService(registration.HandlerType);
                        var handleMethod = registration.HandlerType.GetMethod("HandleAsync");
                        if (handleMethod is null)
                        {
                            throw new InvalidOperationException($"Handler {registration.HandlerType.Name} does not expose HandleAsync method");
                        }

                        var handleTask = (Task)handleMethod.Invoke(handler, new[] { payload, context, cancellationToken })!;
                        await handleTask.ConfigureAwait(false);

                        await _store.StoreInboxAsync(new InboxMessage
                        {
                            ConsumerGroup = registration.GroupId,
                            MessageId = messageId,
                            Topic = registration.Topic,
                            ReceivedAt = DateTimeOffset.UtcNow
                        }, cancellationToken).ConfigureAwait(false);

                        await _store.StoreOffsetAsync(registration.GroupId, registration.Topic, result.Partition.Value, result.Offset.Value, cancellationToken).ConfigureAwait(false);

                        TryCommit(consumer, result, logger);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error processing message from {Topic}", registration.Topic);

                        await _store.StoreDeadLetterAsync(new DeadLetterMessage
                        {
                            Topic = registration.Topic,
                            Key = result.Message.Key,
                            Payload = result.Message.Value,
                            Headers = headers,
                            Reason = ex.Message,
                            FailedAt = DateTimeOffset.UtcNow
                        }, cancellationToken).ConfigureAwait(false);

                        TryCommit(consumer, result, logger);
                    }
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                consumer.Close();
            }
        }

        private static void TryCommit(IConsumer<string?, string> consumer, ConsumeResult<string?, string> result, ILogger logger)
        {
            try
            {
                consumer.Commit(result);
            }
            catch (KafkaException ex)
            {
                logger.LogWarning(ex, "Failed to commit offset for {Topic}", result.Topic);
            }
        }

        private static IDictionary<string, string?> ConvertHeaders(Headers? headers)
        {
            var dictionary = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
            if (headers is null)
            {
                return dictionary;
            }

            foreach (var header in headers)
            {
                dictionary[header.Key] = header.GetValueBytes() is { } bytes ? System.Text.Encoding.UTF8.GetString(bytes) : null;
            }

            if (!dictionary.ContainsKey("message-id"))
            {
                dictionary["message-id"] = Guid.NewGuid().ToString();
            }

            return dictionary;
        }
    }
}
