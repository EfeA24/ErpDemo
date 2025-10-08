using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Entities
{
    public class DeadLetterMessage
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString()!;

        public string Topic { get; set; } = string.Empty;

        public string? Key { get; set; }

        public string Payload { get; set; } = string.Empty;

        public IDictionary<string, string?> Headers { get; set; } = new Dictionary<string, string?>();

        public string Reason { get; set; } = string.Empty;

        public DateTimeOffset FailedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
