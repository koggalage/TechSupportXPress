using TechSupportXPress.Models;
using TechSupportXPress.ViewModels;

namespace TechSupportXPress.Services.Interfaces
{
    public interface ISystemCodeDetailService
    {
        Task<SystemCodeDetailViewModel> GetFilteredAsync(SystemCodeDetailViewModel vm);
        Task<SystemCodeDetail?> GetByIdAsync(int id);

    }
}
