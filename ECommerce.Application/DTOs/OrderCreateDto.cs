using ECommerce.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.DTOs
{
    public class OrderCreateDto
    {
        public Address  ShippingAddress { get; set; }
    }
}
