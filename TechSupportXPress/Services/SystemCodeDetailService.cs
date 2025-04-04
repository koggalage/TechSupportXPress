using Microsoft.EntityFrameworkCore;
using TechSupportXPress.Models;
using TechSupportXPress.Repositories.Interfaces;
using TechSupportXPress.Services.Interfaces;
using TechSupportXPress.ViewModels;

namespace TechSupportXPress.Services
{
    public class SystemCodeDetailService : ISystemCodeDetailService
    {
        private readonly ISystemCodeDetailRepository _repo;

        public SystemCodeDetailService(ISystemCodeDetailRepository repo)
        {
            _repo = repo;
        }

        public async Task<SystemCodeDetailViewModel> GetFilteredAsync(SystemCodeDetailViewModel vm)
        {
            var query = _repo.GetAllWithIncludes();

            if (!string.IsNullOrEmpty(vm.Code))
                query = query.Where(x => x.Code.Contains(vm.Code));

            if (!string.IsNullOrEmpty(vm.CreatedById))
                query = query.Where(x => x.CreatedById == vm.CreatedById);

            if (!string.IsNullOrEmpty(vm.Description))
                query = query.Where(x => x.Description == vm.Description);

            if (vm.SystemCodeId > 0)
                query = query.Where(x => x.SystemCodeId == vm.SystemCodeId);

            vm.SystemCodeDetails = await query.ToListAsync();
            return vm;
        }

        public async Task<SystemCodeDetail?> GetByIdAsync(int id)
        {
            return await _repo.GetByIdWithAuditTrailAsync(id);
        }


    }

}
