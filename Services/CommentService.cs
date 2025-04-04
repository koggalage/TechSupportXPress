using TechSupportXPress.Models;
using TechSupportXPress.Repositories.Interfaces;
using TechSupportXPress.Services.Interfaces;

namespace TechSupportXPress.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepo;

        public CommentService(ICommentRepository commentRepo)
        {
            _commentRepo = commentRepo;
        }

        public async Task AddCommentAsync(int ticketId, string userId, string description)
        {
            var comment = new Comment
            {
                TicketId = ticketId,
                CreatedById = userId,
                CreatedOn = DateTime.Now,
                Description = description
            };

            await _commentRepo.AddAsync(comment);
        }
    }

}
