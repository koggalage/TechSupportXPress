using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using TechSupportXPress.Brokers;
using TechSupportXPress.Data;
using TechSupportXPress.Services.Interfaces;

namespace TechSupportXPress.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<NotificationHub> _hub;

        public NotificationService(ApplicationDbContext context, IHubContext<NotificationHub> hub)
        {
            _context = context;
            _hub = hub;
        }

        public async Task NotifyAdminsAsync(string message)
        {
            var adminRoleId = await _context.Roles
                .Where(r => r.Name == "ADMIN")
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            var adminUsers = await _context.Users
                .Where(u => _context.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == adminRoleId))
                .ToListAsync();

            foreach (var admin in adminUsers)
            {
                await _hub.Clients.User(admin.Id).SendAsync("ReceiveNotification", message);
            }
        }

        public async Task NotifyUserAsync(string userId, string message)
        {
            await _hub.Clients.User(userId).SendAsync("ReceiveNotification", message);
        }


    }
}
