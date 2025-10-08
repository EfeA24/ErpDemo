using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Entities
{
    public class OutboxMessage
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString()!;

        public string Topic { get; set; } = string.Empty;

        public string? Key { get; set; }

        public string Payload { get; set; } = string.Empty;

        public IDictionary<string, string?> Headers { get; set; } = new Dictionary<string, string?>();

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset? PublishedAt { get; set; }

        public int AttemptCount { get; set; }

        public string? LastError { get; set; }
    }
}
