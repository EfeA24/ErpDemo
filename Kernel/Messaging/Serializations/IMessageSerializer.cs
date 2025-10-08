using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Serializations
{
    public interface IMessageSerializer
    {
        string Serialize<T>(T message, IDictionary<string, string?> headers);

        T? Deserialize<T>(string payload, IDictionary<string, string?> headers);

        object? Deserialize(string payload, IDictionary<string, string?> headers, Type type);
    }
}
