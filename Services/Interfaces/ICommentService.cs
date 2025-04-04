namespace TechSupportXPress.Services.Interfaces
{
    public interface ICommentService
    {
        Task AddCommentAsync(int ticketId, string userId, string description);

    }
}
