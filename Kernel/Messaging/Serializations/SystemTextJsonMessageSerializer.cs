using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Messaging.Serializations
{
    public sealed class SystemTextJsonMessageSerializer : IMessageSerializer
    {
        private readonly JsonSerializerOptions _options;

        public SystemTextJsonMessageSerializer(JsonSerializerOptions? options = null)
        {
            _options = options ?? new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };
        }

        public string Serialize<T>(T message, IDictionary<string, string?> headers)
        {
            headers["content-type"] = "application/json";
            headers["message-type"] = typeof(T).AssemblyQualifiedName;
            return JsonSerializer.Serialize(message, _options);
        }

        public T? Deserialize<T>(string payload, IDictionary<string, string?> headers)
        {
            return JsonSerializer.Deserialize<T>(payload, _options);
        }

        public object? Deserialize(string payload, IDictionary<string, string?> headers, Type type)
        {
            return JsonSerializer.Deserialize(payload, type, _options);
        }
    }
}
