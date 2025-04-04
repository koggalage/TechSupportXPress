using TechSupportXPress.Models;

namespace TechSupportXPress.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<List<ApplicationUser>> GetUsersByRoleAsync(string roleName);
        Task<IList<string>> GetUserRolesAsync(ApplicationUser user);
        Task<List<ApplicationUser>> GetAllUsersAsync();


    }
}
