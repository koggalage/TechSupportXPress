using Microsoft.EntityFrameworkCore;
using TechSupportXPress.Models;
using TechSupportXPress.Repositories.Interfaces;
using TechSupportXPress.Services.Interfaces;
using TechSupportXPress.ViewModels;

namespace TechSupportXPress.Services
{
    public class SystemCodeService : ISystemCodeService
    {
        private readonly ISystemCodeRepository _repo;

        public SystemCodeService(ISystemCodeRepository repo)
        {
            _repo = repo;
        }

        public Task<SystemCodeDetail> GetResolutionStatusByDescriptionAsync(string description)
        {
            return _repo.GetByCodeAndDescriptionAsync("ResolutionStatus", description);
        }

        public Task<SystemCodeDetail> GetResolutionStatusByCodeAsync(string code)
        {
            return _repo.GetByCodeAsync("ResolutionStatus", code);
        }

        public Task<List<SystemCodeDetail>> GetResolutionStatusesByCodesAsync(params string[] codes)
        {
            return _repo.GetByCodeListAsync("ResolutionStatus", codes.ToList());
        }

        public async Task<SystemCodeViewModel> GetFilteredAsync(SystemCodeViewModel vm)
        {
            var query = _repo.GetAllWithCreatedBy();

            if (!string.IsNullOrEmpty(vm.Code))
                query = query.Where(x => x.Code.Contains(vm.Code));

            if (!string.IsNullOrEmpty(vm.CreatedById))
                query = query.Where(x => x.CreatedById == vm.CreatedById);

            if (!string.IsNullOrEmpty(vm.Description))
                query = query.Where(x => x.Description == vm.Description);

            vm.SystemCodes = await query.ToListAsync();
            return vm;
        }

        public async Task<SystemCode?> GetByIdAsync(int id)
        {
            return await _repo.GetByIdWithAuditTrailAsync(id);
        }


    }
}
