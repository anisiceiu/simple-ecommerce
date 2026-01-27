namespace simple_ecommerce.Hubs
{
    using ECommerce.Domain;
    using ECommerce.Domain.Entities;
    using ECommerce.Infrastructure;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.SignalR;
    using System.Security.Claims;

    public class ChatHub : Hub
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public ChatHub(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task JoinRoom(string roomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
        }

        public async Task SendMessage(string roomId, string message)
        {
            var userId = Context.UserIdentifier;

            // Save message
            var msg = new Message
            {
                ChatRoomId = int.Parse(roomId),
                SenderId = userId,
                MessageText = message,
                IsRead = false
            };
            _context.Messages.Add(msg);
            await _context.SaveChangesAsync();

            // Send to chat room group (user + admin in that room)
            await Clients.Group(roomId).SendAsync("ReceiveMessage", userId, message, msg.SentAt.ToString("HH:mm"));

            // Notify admin if AdminId exists
            var room = _context.ChatRooms.FirstOrDefault(r => r.Id == int.Parse(roomId));

            if (room != null)
            {
                // Assign AdminId if null (first admin opening the chat)
                if (string.IsNullOrEmpty(room.AdminId))
                {
                    // Get first user in "Admin" role
                    var admins = await _userManager.GetUsersInRoleAsync("Admin"); // synchronous for demo
                    var firstAdmin = admins.FirstOrDefault();
                    if (firstAdmin != null)
                    {
                        room.AdminId = firstAdmin.Id;
                        await _context.SaveChangesAsync();
                    }
                }

                if (!string.IsNullOrEmpty(room.AdminId) && room.AdminId != userId)
                {
                    await Clients.User(room.AdminId).SendAsync("NewChatNotification", roomId, message);
                }
            }
        }

    }

}
