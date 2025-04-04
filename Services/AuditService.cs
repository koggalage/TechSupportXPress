using TechSupportXPress.Models;
using TechSupportXPress.Repositories.Interfaces;
using TechSupportXPress.Services.Interfaces;

namespace TechSupportXPress.Services
{
    public class AuditService : IAuditService
    {
        private readonly IAuditTrailRepository _repo;
        private readonly IHttpContextAccessor _http;

        public AuditService(IAuditTrailRepository repo, IHttpContextAccessor http)
        {
            _repo = repo;
            _http = http;
        }

        public async Task LogAsync(string action, string userId, string module, string table, string ipAddress = null)
        {
            var audit = new AuditTrail
            {
                Action = action,
                TimeStamp = DateTime.Now,
                IpAddress = ipAddress ?? _http.HttpContext?.Connection.RemoteIpAddress?.ToString(),
                UserId = userId,
                Module = module,
                AffectedTable = table
            };

            await _repo.AddAuditAsync(audit);
        }
    }
}
