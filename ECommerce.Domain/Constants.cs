using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain
{
    public static class OrderStatuses
    {
        public const string Pending = "Pending";
        public const string Confirmed = "Confirmed";
        public const string Processing = "Processing";
        public const string Delivered = "Delivered";
        public const string Cancelled = "Cancelled";
    }

}
