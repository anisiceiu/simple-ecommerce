using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain
{
    public enum InventoryTransactionType
    {
        StockIn = 1,
        StockOut = 2,
        Adjustment = 3,
        Purchase = 4,
        Sale = 5,
        Return = 6,
        Damage = 7,
        Transfer = 8
    }
}
