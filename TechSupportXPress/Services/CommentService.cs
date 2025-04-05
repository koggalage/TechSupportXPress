using System.Net;
using TechSupportXPress.Models;
using TechSupportXPress.Repositories.Interfaces;
using TechSupportXPress.Services.Interfaces;

namespace TechSupportXPress.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepo;

        public CommentService(
            ICommentRepository commentRepo
            )
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

        public async Task<List<Comment>> GetAllAsync()
        {
            return await _commentRepo.GetAllWithIncludesAsync();
        }

        public async Task<List<Comment>> GetByTicketIdAsync(int ticketId)
        {
            return await _commentRepo.GetByTicketIdWithIncludesAsync(ticketId);
        }

        public async Task<Comment?> GetByIdAsync(int id)
        {
            return await _commentRepo.GetByIdWithIncludesAsync(id);
        }

        public async Task<bool> CreateAsync(Comment comment)
        {
            comment.CreatedOn = DateTime.Now;

            await _commentRepo.AddAsync(comment);

            return true;
        }

        public async Task<bool> UpdateAsync(Comment comment)
        {
            await _commentRepo.UpdateAsync(comment);

            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _commentRepo.ExistsAsync(id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var comment = await _commentRepo.GetByIdAsync(id);
            if (comment == null)
                return false;

            await _commentRepo.DeleteAsync(comment);

            return true;
        }


    }

}
