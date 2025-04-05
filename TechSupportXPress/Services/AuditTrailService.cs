using TechSupportXPress.Models;
using TechSupportXPress.Repositories.Interfaces;
using TechSupportXPress.Services.Interfaces;

namespace TechSupportXPress.Services
{
    public class AuditTrailService : IAuditTrailService
    {
        private readonly IAuditTrailRepository _repo;
        private readonly IHttpContextAccessor _http;

        public AuditTrailService(IAuditTrailRepository repo, IHttpContextAccessor http)
        {
            _repo = repo;
            _http = http;
        }

        public async Task LogAsync(string action, string userId, string module, string table, string ipAddress = null, string? oldValues = null, string? newValues = null)
        {
            var audit = new AuditTrail
            {
                Action = action,
                TimeStamp = DateTime.Now,
                IpAddress = ipAddress ?? _http.HttpContext?.Connection.RemoteIpAddress?.ToString(),
                UserId = userId,
                Module = module,
                AffectedTable = table,
                OldValues = oldValues,
                NewValues = newValues
            };

            await _repo.AddAuditAsync(audit);
        }


        public async Task<List<AuditTrail>> GetAllAsync()
        {
            return await _repo.GetAllWithUsersAsync();
        }

        public async Task<AuditTrail?> GetByIdAsync(int id)
        {
            return await _repo.GetByIdWithUserAsync(id);
        }

    }
}
