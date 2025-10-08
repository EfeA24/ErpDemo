using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockService.Core.Domain.Enums
{
    public enum InventoryReferenceType
    {
        None = 0,
        SalesOrder = 1,
        PurchaseOrder = 2,
        Shipment = 3,
        ProductionOrder = 4,
        StockCount = 5
    }
}
