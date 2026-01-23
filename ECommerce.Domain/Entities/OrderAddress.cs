using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Entities
{
    public class OrderAddress
    {
        public int Id { get; set; }
        public int OrderId { get; set; }

        public string FullName { get; set; }
        public string Phone { get; set; }
        public string AddressLine { get; set; }
        public string City { get; set; }
        public Order Order { get; set; }
    }
}
