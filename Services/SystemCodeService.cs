using TechSupportXPress.Models;
using TechSupportXPress.Repositories.Interfaces;
using TechSupportXPress.Services.Interfaces;

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

    }
}
