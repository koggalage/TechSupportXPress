using TechSupportXPress.Models;

namespace TechSupportXPress.Services.Interfaces
{
    public interface IAuditTrailService
    {
        Task LogAsync(string action, string userId, string module, string table, string ipAddress);
        Task<List<AuditTrail>> GetAllAsync();
        Task<AuditTrail?> GetByIdAsync(int id);

    }
}
