using Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockService.Core.Domain.Entities.Warehouse
{
    public class Warehouse : BaseEntity
    {
        public string? WarehouseCode { get; set; }
        public string? Description { get; set; } = null;
    }
}
