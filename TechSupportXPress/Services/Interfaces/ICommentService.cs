using TechSupportXPress.Models;

namespace TechSupportXPress.Services.Interfaces
{
    public interface ICommentService
    {
        Task AddCommentAsync(int ticketId, string userId, string description);
        Task<List<Comment>> GetAllAsync();
        Task<List<Comment>> GetByTicketIdAsync(int ticketId);
        Task<Comment?> GetByIdAsync(int id);
        Task<bool> CreateAsync(Comment comment);
        Task<bool> UpdateAsync(Comment comment);
        Task<bool> ExistsAsync(int id);
        Task<bool> DeleteAsync(int id);

    }
}
