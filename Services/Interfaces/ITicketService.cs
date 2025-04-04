using System.Security.Claims;
using TechSupportXPress.Models;
using TechSupportXPress.ViewModels;

namespace TechSupportXPress.Services.Interfaces
{
    public interface ITicketService
    {
        Task<TicketViewModel> GetFilteredTicketsAsync(string userId, IList<string> roles, TicketViewModel filterVm);
        Task<TicketViewModel> GetTicketDetailsViewModelAsync(int ticketId);
        Task<int> CreateTicketAsync(TicketCreateViewModel vm, string userId, string attachmentFileName);
        Task<TicketViewModel> GetTicketEditViewModelAsync(int id);
        Task<bool> UpdateTicketAsync(int id, TicketViewModel model, IFormFile attachment, string userId);

        Task<Ticket> GetTicketForDeleteViewAsync(int id);
        Task<bool> DeleteTicketAsync(int id);

        Task<TicketViewModel> GetTicketResolveViewModelAsync(int ticketId);
        Task<bool> ResolveTicketAsync(int ticketId, int newStatusId, string description, string userId);
        Task<Ticket> GetTicketByIdAsync(int id);

        Task<TicketViewModel> GetTicketCloseViewModelAsync(int ticketId);
        Task<bool> CloseTicketAsync(int ticketId, string userId);
        Task<TicketViewModel> GetTicketReOpenViewModelAsync(int ticketId);
        Task<bool> ReOpenTicketAsync(int ticketId, string userId);
        Task<TicketViewModel> GetTicketAssignmentViewModelAsync(int ticketId);
        Task<bool> AssignTicketAsync(int ticketId, string assignedToId, string userId);
        Task<TicketViewModel> GetAssignedTicketsAsync(TicketViewModel filterVm, ApplicationUser currentUser);
        Task<TicketViewModel> GetClosedTicketsAsync(TicketViewModel filterVm, ApplicationUser currentUser);
        Task<TicketViewModel> GetResolvedTicketsAsync(TicketViewModel filterVm, ApplicationUser currentUser);

    }
}
