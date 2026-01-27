using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Entities
{
    public class ChatRoom
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string? AdminId { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsClosed { get; set; }

        public ICollection<Message> Messages { get; set; }
    }

}
