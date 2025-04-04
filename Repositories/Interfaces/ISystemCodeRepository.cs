using TechSupportXPress.Models;

namespace TechSupportXPress.Repositories.Interfaces
{
    public interface ISystemCodeRepository
    {
        Task<SystemCodeDetail> GetByCodeAndDescriptionAsync(string systemCode, string description);
        Task<SystemCodeDetail> GetByCodeAsync(string systemCode, string code);
        Task<List<SystemCodeDetail>> GetByCodeListAsync(string systemCode, List<string> codeValues);

    }
}
