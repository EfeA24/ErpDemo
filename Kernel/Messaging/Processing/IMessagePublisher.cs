using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Processing
{
    public interface IMessagePublisher
    {
        Task PublishAsync<T>(string topic, string? key, T message, IDictionary<string, string?>? headers = null, CancellationToken cancellationToken = default);
    }
}
