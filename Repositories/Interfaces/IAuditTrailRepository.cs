using TechSupportXPress.Models;

namespace TechSupportXPress.Repositories.Interfaces
{
    public interface IAuditTrailRepository
    {
        Task AddAuditAsync(AuditTrail auditTrail);
    }
}
