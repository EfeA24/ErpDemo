using Shared.Entities;
using StockService.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockService.Core.Domain.Entities.Inventory
{
    public class InventoryTransaction : BaseEntity
    {
        public int ProductId { get; set; }
        public int WarehouseId { get; set; }
        public int? FromLocationId { get; set; }
        public int? ToLocationId { get; set; }
        public int? LocationId { get; set; }

        public InventoryTransactionType Type { get; set; }
        public DateTime TxDate { get; set; }
        public decimal Quantity { get; set; }
        public string? UomCode { get; set; }

        public InventoryReferenceType ReferenceType { get; set; } = InventoryReferenceType.None;
        public int? ReferenceId { get; set; }
        public string? ReferenceNo { get; set; }

        public string? ReasonCode { get; set; }
        public string? Note { get; set; }
    }
}
