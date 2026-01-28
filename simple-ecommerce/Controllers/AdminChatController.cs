using ECommerce.Domain.Entities;
using ECommerce.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ecommerce.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminChatController : Controller
    {
        private readonly AppDbContext _context;

        public AdminChatController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var chats = _context.ChatRooms
                .Include(x => x.Messages)
                .Where(x => !x.IsClosed)
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            return View(chats);
        }

        public IActionResult Open(int id)
        {
            // Load chat with messages
            var chat = _context.ChatRooms
                .Include(x => x.Messages)
                .FirstOrDefault(x => x.Id == id);

            if (chat == null)
            {
                // Return 404 if chat not found
                return NotFound("Chat room not found.");
            }

            // Ensure Messages is not null
            chat.Messages = chat.Messages ?? new List<Message>();

            // Get admin id safely
            var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(adminId))
            {
                // Mark unread messages as read
                var unread = chat.Messages.Where(m => !m.IsRead && m.SenderId != adminId).ToList();
                foreach (var msg in unread)
                {
                    msg.IsRead = true;
                }
                _context.SaveChanges();
            }

            return View(chat);
        }

    }

}
