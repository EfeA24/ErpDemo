using Confluent.Kafka;
using Messaging.Configurations;
using Messaging.Storage;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Processing
{
    public class OutboxPublisherService : BackgroundService
    {
        private readonly KafkaProducerFactory _producerFactory;
        private readonly IMongoMessageStore _store;
        private readonly ILogger<OutboxPublisherService> _logger;
        private readonly KafkaOptions _options;

        public OutboxPublisherService(KafkaProducerFactory producerFactory, IMongoMessageStore store, IOptions<KafkaOptions> options, ILogger<OutboxPublisherService> logger)
        {
            _producerFactory = producerFactory;
            _store = store;
            _logger = logger;
            _options = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await DispatchBatchAsync(stoppingToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error dispatching outbox messages");
                }

                await Task.Delay(_options.OutboxDispatchInterval, stoppingToken).ConfigureAwait(false);
            }
        }

        private async Task DispatchBatchAsync(CancellationToken cancellationToken)
        {
            var messages = await _store.GetUnpublishedOutboxAsync(_options.OutboxBatchSize, cancellationToken).ConfigureAwait(false);
            if (messages.Count == 0)
            {
                return;
            }

            var producer = _producerFactory.CreateProducer();

            foreach (var message in messages)
            {
                var kafkaMessage = new Message<string?, string>
                {
                    Key = message.Key,
                    Value = message.Payload,
                    Headers = BuildHeaders(message.Headers)
                };

                try
                {
                    await producer.ProduceAsync(message.Topic, kafkaMessage, cancellationToken).ConfigureAwait(false);
                    await _store.MarkOutboxPublishedAsync(message, cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to publish outbox message {MessageId}", message.Id);
                    await _store.MarkOutboxFailedAsync(message, ex.Message, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        private static Headers BuildHeaders(IDictionary<string, string?> source)
        {
            var headers = new Headers();
            foreach (var kvp in source)
            {
                if (kvp.Value is null)
                {
                    continue;
                }

                headers.Add(kvp.Key, System.Text.Encoding.UTF8.GetBytes(kvp.Value));
            }

            return headers;
        }
    }
}
