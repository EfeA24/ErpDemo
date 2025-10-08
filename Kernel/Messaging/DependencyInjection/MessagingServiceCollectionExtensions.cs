using Messaging.Configurations;
using Messaging.Processing;
using Messaging.Serializations;
using Messaging.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.DependencyInjection
{
    public static class MessagingServiceCollectionExtensions
    {
        public static MessagingBuilder AddMessaging(this IServiceCollection services, Action<MessagingBuilder>? configure = null)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddOptions<KafkaOptions>();
            services.AddOptions<MongoOptions>();

            services.TryAddSingleton<IMessageSerializer, SystemTextJsonMessageSerializer>();
            services.TryAddSingleton<IMongoClient>(provider =>
            {
                var mongoOptions = provider.GetRequiredService<IOptions<MongoOptions>>().Value;
                return new MongoClient(mongoOptions.ConnectionString);
            });
            services.TryAddSingleton<IMongoMessageStore, MongoMessageStore>();
            services.TryAddSingleton<KafkaProducerFactory>();
            services.TryAddSingleton<IMessagePublisher, KafkaMessagePublisher>();

            services.AddHostedService<OutboxPublisherService>();
            services.AddHostedService<KafkaConsumerHostedService>();

            var builder = new MessagingBuilder(services);
            configure?.Invoke(builder);

            services.AddSingleton<IEnumerable<KafkaConsumerRegistration>>(_ => builder.Registrations);

            return builder;
        }
    }
}
