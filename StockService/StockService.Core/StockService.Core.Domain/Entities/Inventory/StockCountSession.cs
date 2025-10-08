using Shared.Entities;
using StockService.Core.Domain.Entities.Warehouse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockService.Core.Domain.Entities.Inventory
{
    public class StockCountSession : BaseEntity
    {
        public Guid WarehouseId { get; set; }
        public Warehouse.Warehouse Warehouse { get; set; } = null!;
        public string Code { get; set; } = string.Empty;
        public DateTime PlannedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        public ICollection<StockCountItem> Items { get; set; } = new List<StockCountItem>();
    }
}
