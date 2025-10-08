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
        public decimal PurchasePrice { get; set; }
        public decimal RetailPrice { get; set; }
        public int MinLevel { get; set; }
        public int OrderedQty { get; set; }

        public Guid LocationId { get; set; }
        public Location? Location { get; set; }
        public Guid WarehouseId { get; set; }
        public Warehouse.Warehouse? Warehouse { get; set; }

    }
}
