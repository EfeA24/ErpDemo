using Shared.Entities;
using StockService.Core.Domain.Entities.Warehouse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockService.Core.Domain.Entities.Inventory
{
    public class StockItem : BaseEntity
    {
        public string Description { get; set; } = null!;
        public int Quantity { get; set; }
        public int OnHand { get; set; }
        public int Reserved { get; set; }
        public int Available => OnHand - Reserved;
        public int MinLevel { get; set; }
        public int Ordered { get; set; }

        public Guid LocationId { get; set; }
        public Location? Location { get; set; }
        public Guid WarehouseId { get; set; }
        public Warehouse.Warehouse? Warehouse { get; set; }

    }
}
