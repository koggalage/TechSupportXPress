using TechSupportXPress.Models;

namespace TechSupportXPress.Repositories.Interfaces
{
    public interface ICommentRepository
    {
        Task<List<Comment>> GetCommentsByTicketIdAsync(int ticketId);
        Task AddAsync(Comment comment);

    }
}
