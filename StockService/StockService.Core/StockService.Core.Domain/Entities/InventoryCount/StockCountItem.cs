using Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockService.Core.Domain.Entities.InventoryCount
{
    public class StockCountItem : BaseEntity
    {
        public int StockCountSessionId { get; set; }
        public int ProductId { get; set; }
        public int? LocationId { get; set; }

        public decimal SystemQty { get; set; }
        public decimal CountedQty { get; set; }
        public decimal Difference => CountedQty - SystemQty;

        public string? UomCode { get; set; }
        public string? Note { get; set; }
    }
}
