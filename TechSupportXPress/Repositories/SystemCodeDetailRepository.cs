using Microsoft.EntityFrameworkCore;
using TechSupportXPress.Data;
using TechSupportXPress.Models;
using TechSupportXPress.Repositories.Interfaces;

namespace TechSupportXPress.Repositories
{
    public class SystemCodeDetailRepository : ISystemCodeDetailRepository
    {
        private readonly ApplicationDbContext _context;

        public SystemCodeDetailRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<SystemCodeDetail> GetAllWithIncludes()
        {
            return _context.SystemCodeDetails
                .Include(s => s.SystemCode)
                .Include(s => s.CreatedBy)
                .OrderByDescending(s => s.CreatedOn);
        }

        public async Task<SystemCodeDetail?> GetByIdWithAuditTrailAsync(int id)
        {
            return await _context.SystemCodeDetails
                .Include(s => s.CreatedBy)
                .Include(s => s.ModifiedBy)
                .Include(s => s.DeletedBy)
                .Include(s => s.SystemCode)
                .FirstOrDefaultAsync(s => s.Id == id);
        }


    }
}
