using Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockService.Core.Domain.Entities.InventoryCount
{
    public class StockCountSession : BaseEntity
    {
        public int WarehouseId { get; set; }
        public string Code { get; set; } = string.Empty;
        public DateTime PlannedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        public ICollection<StockCountItem> Items { get; set; } = new List<StockCountItem>();
    }
}
