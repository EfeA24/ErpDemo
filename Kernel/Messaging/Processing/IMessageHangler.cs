using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Processing
{
    public interface IMessageHandler<in TMessage>
    {
        Task HandleAsync(TMessage message, MessageContext context, CancellationToken cancellationToken);
    }
}
