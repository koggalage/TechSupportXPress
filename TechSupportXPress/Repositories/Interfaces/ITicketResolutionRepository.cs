using TechSupportXPress.Models;

namespace TechSupportXPress.Repositories.Interfaces
{
    public interface ITicketResolutionRepository
    {
        Task<List<TicketResolution>> GetResolutionsByTicketIdAsync(int ticketId);
        Task AddAsync(TicketResolution resolution);

    }
}
