using TechSupportXPress.Models;

namespace TechSupportXPress.Repositories.Interfaces
{
    public interface ISystemCodeDetailRepository
    {
        IQueryable<SystemCodeDetail> GetAllWithIncludes();
        Task<SystemCodeDetail?> GetByIdWithAuditTrailAsync(int id);

    }
}
