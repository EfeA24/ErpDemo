using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Entities
{
    public class ConsumerOffset
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString()!;

        public string ConsumerGroup { get; set; } = string.Empty;

        public string Topic { get; set; } = string.Empty;

        public int Partition { get; set; }

        public long Offset { get; set; }
    }
}
