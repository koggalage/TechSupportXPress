using TechSupportXPress.Models;

namespace TechSupportXPress.Repositories.Interfaces
{
    public interface IAuditTrailRepository
    {
        Task AddAuditAsync(AuditTrail auditTrail);
        Task<List<AuditTrail>> GetAllWithUsersAsync();
        Task<AuditTrail?> GetByIdWithUserAsync(int id);

    }
}
