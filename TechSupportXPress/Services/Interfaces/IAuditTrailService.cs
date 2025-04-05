using TechSupportXPress.Models;

namespace TechSupportXPress.Services.Interfaces
{
    public interface IAuditTrailService
    {
        Task LogAsync(string action, string userId, string module, string table, string ipAddress = null, string? oldValues = null, string? newValues = null);
        Task<List<AuditTrail>> GetAllAsync();
        Task<AuditTrail?> GetByIdAsync(int id);

    }
}
