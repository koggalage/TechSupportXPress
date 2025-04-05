using TechSupportXPress.Models;

namespace TechSupportXPress.Repositories.Interfaces
{
    public interface ICommentRepository
    {
        Task<List<Comment>> GetCommentsByTicketIdAsync(int ticketId);
        Task AddAsync(Comment comment);
        Task<List<Comment>> GetAllWithIncludesAsync();
        Task<List<Comment>> GetByTicketIdWithIncludesAsync(int ticketId);
        Task<Comment?> GetByIdWithIncludesAsync(int id);
        Task UpdateAsync(Comment comment);
        Task<bool> ExistsAsync(int id);
        Task DeleteAsync(Comment comment);
        Task<Comment?> GetByIdAsync(int id);
        Task DeleteByTicketIdAsync(int ticketId);

    }
}
