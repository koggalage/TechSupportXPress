using Microsoft.EntityFrameworkCore;
using TechSupportXPress.Data;
using TechSupportXPress.Models;
using TechSupportXPress.Repositories.Interfaces;

namespace TechSupportXPress.Repositories
{
    public class SystemCodeRepository : ISystemCodeRepository
    {
        private readonly ApplicationDbContext _context;

        public SystemCodeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SystemCodeDetail> GetByCodeAndDescriptionAsync(string systemCode, string description)
        {
            return await _context.SystemCodeDetails
                .Include(x => x.SystemCode)
                .FirstOrDefaultAsync(x => x.SystemCode.Code == systemCode && x.Description == description);
        }

        public async Task<SystemCodeDetail> GetByCodeAsync(string systemCode, string code)
        {
            return await _context.SystemCodeDetails
                .Include(x => x.SystemCode)
                .FirstOrDefaultAsync(x => x.SystemCode.Code == systemCode && x.Code == code);
        }

        public async Task<List<SystemCodeDetail>> GetByCodeListAsync(string systemCode, List<string> codeValues)
        {
            return await _context.SystemCodeDetails
                .Include(x => x.SystemCode)
                .Where(x => x.SystemCode.Code == systemCode && codeValues.Contains(x.Code))
                .ToListAsync();
        }

        public IQueryable<SystemCode> GetAllWithCreatedBy()
        {
            return _context.SystemCodes
                .Include(x => x.CreatedBy)
                .OrderByDescending(x => x.CreatedOn);
        }

        public async Task<SystemCode?> GetByIdWithAuditTrailAsync(int id)
        {
            return await _context.SystemCodes
                .Include(s => s.CreatedBy)
                .Include(s => s.ModifiedBy)
                .Include(s => s.DeletedBy)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<List<SystemCode>> GetAllAsync()
        {
            return await _context.SystemCodes
                .OrderBy(x => x.Description)
                .ToListAsync();
        }


    }
}
