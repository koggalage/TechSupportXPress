using System.Net;
using TechSupportXPress.Models;
using TechSupportXPress.Repositories.Interfaces;
using TechSupportXPress.Services.Interfaces;

namespace TechSupportXPress.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepo;
        private readonly IAuditTrailService _auditService;

        public CommentService(
            ICommentRepository commentRepo,
            IAuditTrailService auditService
            )
        {
            _commentRepo = commentRepo;
            _auditService = auditService;
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

        public async Task<bool> CreateAsync(Comment comment, string userId, string ipAddress)
        {
            comment.CreatedOn = DateTime.Now;
            comment.CreatedById = userId;

            await _commentRepo.AddAsync(comment);

            await _auditService.LogAsync(
                action: "Create",
                userId: userId,
                module: "Comments",
                table: "Comments",
                ipAddress
            );

            return true;
        }

        public async Task<bool> UpdateAsync(Comment comment, string userId, string ipAddress)
        {
            await _commentRepo.UpdateAsync(comment);

            await _auditService.LogAsync(
                action: "Update",
                userId: userId,
                module: "Comments",
                table: "Comments",
                ipAddress
            );

            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _commentRepo.ExistsAsync(id);
        }

        public async Task<bool> DeleteAsync(int id, string userId, string? ipAddress)
        {
            var comment = await _commentRepo.GetByIdAsync(id);
            if (comment == null)
                return false;

            await _commentRepo.DeleteAsync(comment);

            await _auditService.LogAsync(
                action: "Delete",
                userId: userId,
                module: "Comments",
                table: "Comments",
                ipAddress: ipAddress
            );

            return true;
        }


    }

}
