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
            if (!int.TryParse(roomId, out int roomIdInt))
                throw new Exception("Invalid roomId");

            var userId = Context.UserIdentifier;
            if (string.IsNullOrEmpty(userId))
                throw new Exception("UserIdentifier is null");

            // Ensure sender joins the group
            await Groups.AddToGroupAsync(Context.ConnectionId, roomIdInt.ToString());

            // Save message
            var msg = new Message
            {
                ChatRoomId = roomIdInt,
                SenderId = userId,
                MessageText = message,
                IsRead = false,
                SentAt = DateTime.Now
            };
            _context.Messages.Add(msg);
            await _context.SaveChangesAsync();

            // Send to chat room group (customer + admin)
            await Clients.Group(roomIdInt.ToString())
                .SendAsync("ReceiveMessage", userId, message, msg.SentAt.ToString("HH:mm"));

            // Notify admin if AdminId exists
            var room = _context.ChatRooms.FirstOrDefault(r => r.Id == roomIdInt);
            if (room != null)
            {
                if (string.IsNullOrEmpty(room.AdminId))
                {
                    var admins = await _userManager.GetUsersInRoleAsync("Admin");
                    var firstAdmin = admins.FirstOrDefault();
                    if (firstAdmin != null)
                    {
                        room.AdminId = firstAdmin.Id;
                        await _context.SaveChangesAsync();
                    }
                }

                if (!string.IsNullOrEmpty(room.AdminId) && room.AdminId != userId)
                {
                    await Clients.User(room.AdminId)
                        .SendAsync("NewChatNotification", roomIdInt.ToString(), message);
                }
            }
        }


    }

}
