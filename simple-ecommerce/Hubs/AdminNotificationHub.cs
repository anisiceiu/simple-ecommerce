namespace simple_ecommerce.Hubs
{
    using Microsoft.AspNetCore.SignalR;

    public class AdminNotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            if (Context.User.IsInRole("Admin"))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
            }

            await base.OnConnectedAsync();
        }
    }

}
