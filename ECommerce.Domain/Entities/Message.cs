using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Entities
{
    public class Message
    {
        public int Id { get; set; }
        public int ChatRoomId { get; set; }
        public string SenderId { get; set; }
        public string MessageText { get; set; }
        public bool IsRead { get; set; }
        public DateTime SentAt { get; set; }

        public ChatRoom ChatRoom { get; set; }
    }

}
