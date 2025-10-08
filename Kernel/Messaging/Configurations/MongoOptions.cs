using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Configurations
{
    public class MongoOptions
    {
        public const string SectionName = "Messaging:Mongo";

        public string ConnectionString { get; set; } = string.Empty;

        public string DatabaseName { get; set; } = string.Empty;

        public string OutboxCollectionName { get; set; } = "outbox";

        public string InboxCollectionName { get; set; } = "inbox";

        public string DeadLetterCollectionName { get; set; } = "dead_letters";

        public string OffsetsCollectionName { get; set; } = "consumer_offsets";
    }
}
