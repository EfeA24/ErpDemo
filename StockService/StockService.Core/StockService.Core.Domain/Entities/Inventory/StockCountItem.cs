using Shared.Entities;
using StockService.Core.Domain.Entities.Inventory;
using StockService.Core.Domain.Entities.Warehouse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockService.Core.Domain.Entities.Inventory
{
    public class StockCountItem : BaseEntity
    {
        public Guid ItemId { get; set; }
        public StockItem? Item { get; set; }
        public Guid LocationId { get; set; }
        public Location? Location { get; set; }

        public decimal SystemQty { get; set; }
        public decimal CountedQty { get; set; }
        public decimal Difference => CountedQty - SystemQty;

        public string? Note { get; set; }
    }
}
