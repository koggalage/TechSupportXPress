using TechSupportXPress.Models;
using TechSupportXPress.ViewModels;

namespace TechSupportXPress.Services.Interfaces
{
    public interface ISystemCodeService
    {
        Task<SystemCodeDetail> GetResolutionStatusByDescriptionAsync(string description);
        Task<SystemCodeDetail> GetResolutionStatusByCodeAsync(string code);
        Task<List<SystemCodeDetail>> GetResolutionStatusesByCodesAsync(params string[] codes);
        Task<SystemCodeViewModel> GetFilteredAsync(SystemCodeViewModel filterVm);
        Task<SystemCode?> GetByIdAsync(int id);

    }
}
