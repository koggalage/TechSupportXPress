using TechSupportXPress.Data;
using TechSupportXPress.Models;
using TechSupportXPress.Repositories.Interfaces;

namespace TechSupportXPress.Repositories
{
    public class AuditTrailRepository : IAuditTrailRepository
    {
        private readonly ApplicationDbContext _context;

        public AuditTrailRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAuditAsync(AuditTrail auditTrail)
        {
            _context.Add(auditTrail);
            await _context.SaveChangesAsync();
        }
    }
}
