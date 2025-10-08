using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockService.Core.Domain.Enums
{
    public enum InventoryTransactionType
    {
        Receipt = 1,     // Giriş
        Issue = 2,       // Çıkış
        Transfer = 3,    // Depo/raf arası
        Adjustment = 4
    }
}
