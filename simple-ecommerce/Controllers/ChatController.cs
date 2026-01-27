using ECommerce.Domain.Entities;
using ECommerce.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace simple_ecommerce.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly AppDbContext _context;

        public ChatController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var room = _context.ChatRooms
                .Include(r => r.Messages)   // important!
                .FirstOrDefault(r => r.UserId == userId && !r.IsClosed);

            if (room == null)
            {
                room = new ChatRoom { UserId = userId };
                _context.ChatRooms.Add(room);
                _context.SaveChanges();
            }

            // Mark unread as read safely
            var unread = room.Messages?.Where(m => !m.IsRead && m.SenderId != userId).ToList();
            if (unread != null)
            {
                foreach (var msg in unread)
                {
                    msg.IsRead = true;
                }
                _context.SaveChanges();
            }

            return View(room);
        }
    }
}
