using Microsoft.EntityFrameworkCore;
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

        public async Task<List<AuditTrail>> GetAllWithUsersAsync()
        {
            return await _context.AuditTrails
                .Include(a => a.User)
                .OrderByDescending(a => a.TimeStamp)
                .ToListAsync();
        }

        public async Task<AuditTrail?> GetByIdWithUserAsync(int id)
        {
            return await _context.AuditTrails
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Id == id);
        }


    }
}
