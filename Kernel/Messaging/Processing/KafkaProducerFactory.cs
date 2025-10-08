using Confluent.Kafka;
using Messaging.Configurations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Processing
{
    public class KafkaProducerFactory : IDisposable
    {
        private readonly IProducer<string?, string> _producer;

        public KafkaProducerFactory(IOptions<KafkaOptions> options, ILogger<KafkaProducerFactory> logger)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = options.Value.BootstrapServers,
                ClientId = options.Value.ClientId
            };

            _producer = new ProducerBuilder<string?, string>(config)
                .SetErrorHandler((_, error) => logger.LogError("Kafka producer error: {Error}", error))
                .Build();
        }

        public IProducer<string?, string> CreateProducer() => _producer;

        public void Dispose()
        {
            _producer.Flush(TimeSpan.FromSeconds(5));
            _producer.Dispose();
        }
    }
}
