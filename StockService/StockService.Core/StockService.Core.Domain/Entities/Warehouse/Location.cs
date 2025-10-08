using Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockService.Core.Domain.Entities.Warehouse
{
    public class Location : BaseEntity
    {
        public string LocationCode { get; set; } = null!;
        public bool IsPickable { get; set; } = true;
        public bool IsReceivable { get; set; } = true;

        public Guid WarehouseId { get; set; }
        public Warehouse? Warehouse { get; set; }
    }
}
