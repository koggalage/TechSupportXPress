using Microsoft.AspNetCore.Mvc.Rendering;

namespace TechSupportXPress.Services.Interfaces
{
    public interface IDropdownService
    {
        Task<List<SelectListItem>> GetPrioritiesAsync();
        Task<List<SelectListItem>> GetCategoriesAsync();
        Task<List<SelectListItem>> GetStatusesAsync();
        Task<List<SelectListItem>> GetUsersAsync();
        Task<List<SelectListItem>> GetSubCategoriesByCategoryIdAsync(int categoryId);
        Task<List<SelectListItem>> GetResolutionStatusesAsync();
        Task<List<SelectListItem>> GetSupportUsersAsync();

    }
}
