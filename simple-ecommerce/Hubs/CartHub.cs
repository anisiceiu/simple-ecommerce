using Microsoft.AspNetCore.SignalR;
using ecommerce.Models;
using System.Collections.Generic;

namespace ecommerce.Hubs
{
    public class CartHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            // Get the user ID from context
            var userId = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value 
                      ?? Context.ConnectionId;

            // Add user to a group based on their identity
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");

            await base.OnConnectedAsync();
        }

        /// <summary>
        /// Notifies the user's cart group that an item was added
        /// </summary>
        public async Task NotifyItemAdded(CartItemViewModel item)
        {
            var userId = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value 
                      ?? Context.ConnectionId;

            await Clients.Group($"user-{userId}")
                .SendAsync("ItemAdded", item);
        }

        /// <summary>
        /// Notifies the user's cart group that an item was removed
        /// </summary>
        public async Task NotifyItemRemoved(int productId)
        {
            var userId = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value 
                      ?? Context.ConnectionId;

            await Clients.Group($"user-{userId}")
                .SendAsync("ItemRemoved", productId);
        }

        /// <summary>
        /// Notifies the user's cart group that an item quantity was updated
        /// </summary>
        public async Task NotifyItemUpdated(int productId, int quantity)
        {
            var userId = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value 
                      ?? Context.ConnectionId;

            await Clients.Group($"user-{userId}")
                .SendAsync("ItemUpdated", new { productId, quantity });
        }

        /// <summary>
        /// Notifies the user's cart group that the cart was cleared
        /// </summary>
        public async Task NotifyCartCleared()
        {
            var userId = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value 
                      ?? Context.ConnectionId;

            await Clients.Group($"user-{userId}")
                .SendAsync("CartCleared");
        }

        /// <summary>
        /// Sends the complete cart to all clients in the user's group
        /// </summary>
        public async Task SendFullCart(List<CartItemViewModel> cart)
        {
            var userId = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value 
                      ?? Context.ConnectionId;

            await Clients.Group($"user-{userId}")
                .SendAsync("CartUpdated", cart);
        }
    }
}
