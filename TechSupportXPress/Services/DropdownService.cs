using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechSupportXPress.Data;
using TechSupportXPress.Repositories.Interfaces;
using TechSupportXPress.Services.Interfaces;

namespace TechSupportXPress.Services
{
    public class DropdownService : IDropdownService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserRepository _userRepository;

        public DropdownService(
            ApplicationDbContext context,
            IUserRepository userRepository
            )
        {
            _context = context;
            _userRepository = userRepository;
        }

        public async Task<List<SelectListItem>> GetPrioritiesAsync()
        {
            return await _context.SystemCodeDetails
                .Include(x => x.SystemCode)
                .Where(x => x.SystemCode.Code == "Priority")
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Description })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetCategoriesAsync()
        {
            return await _context.TicketCategories
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetStatusesAsync()
        {
            return await _context.SystemCodeDetails
                .Include(x => x.SystemCode)
                .Where(x => x.SystemCode.Code == "ResolutionStatus")
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Description })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetUsersAsync()
        {
            return await _context.Users
                .Select(x => new SelectListItem { Value = x.Id, Text = x.FullName })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetSubCategoriesByCategoryIdAsync(int categoryId)
        {
            return await _context.TicketSubCategories
                .Where(x => x.CategoryId == categoryId)
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetResolutionStatusesAsync()
        {
            return await _context.SystemCodeDetails
                .Include(x => x.SystemCode)
                .Where(x => x.SystemCode.Code == "ResolutionStatus")
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Description })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetSupportUsersAsync()
        {
            var supportUsers = await _userRepository.GetUsersByRoleAsync("SUPPORT");

            return supportUsers.Select(u => new SelectListItem
            {
                Value = u.Id,
                Text = u.FullName
            }).ToList();
        }


    }
}
