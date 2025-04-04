using TechSupportXPress.Models;
using TechSupportXPress.ViewModels;

namespace TechSupportXPress.Repositories.Interfaces
{
    public interface ITicketRepository
    {
        IQueryable<Ticket> GetAllTicketsWithIncludes();
        Task<Ticket> GetTicketDetailsByIdAsync(int id);
        Task<int> AddAsync(Ticket ticket);
        Task<Ticket> GetTicketWithSubCategoryAsync(int id);
        Task<Ticket> GetByIdAsync(int id);
        Task UpdateAsync(Ticket ticket);
        bool TicketExists(int id);
        Task<Ticket> GetWithCreatedByAsync(int id);
        Task DeleteAsync(Ticket ticket);
        Task<Ticket> GetTicketForResolveAsync(int id);
        Task<List<Ticket>> GetTicketsByStatusIdsAsync(List<int> statusIds);


    }

}
