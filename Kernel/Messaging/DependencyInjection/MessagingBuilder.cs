using Messaging.Configurations;
using Messaging.Processing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.DependencyInjection
{
    public sealed class MessagingBuilder
    {
        private readonly IServiceCollection _services;
        private readonly List<KafkaConsumerRegistration> _registrations = new();

        internal MessagingBuilder(IServiceCollection services)
        {
            _services = services;
        }

        internal IReadOnlyCollection<KafkaConsumerRegistration> Registrations => _registrations;

        public MessagingBuilder ConfigureKafka(Action<KafkaOptions> configure)
        {
            _services.AddOptions<KafkaOptions>().Configure(configure);
            return this;
        }

        public MessagingBuilder ConfigureMongo(Action<MongoOptions> configure)
        {
            _services.AddOptions<MongoOptions>().Configure(configure);
            return this;
        }

        public MessagingBuilder AddConsumer<TMessage, THandler>(string topic, string groupId)
            where THandler : class, IMessageHandler<TMessage>
        {
            _services.AddScoped<THandler>();
            _services.AddScoped(typeof(IMessageHandler<TMessage>), typeof(THandler));

            _registrations.Add(new KafkaConsumerRegistration
            {
                Topic = topic,
                GroupId = groupId,
                MessageType = typeof(TMessage),
                HandlerType = typeof(THandler)
            });

            return this;
        }
    }
}
