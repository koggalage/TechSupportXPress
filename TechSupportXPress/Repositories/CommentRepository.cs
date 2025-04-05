using Microsoft.EntityFrameworkCore;
using TechSupportXPress.Data;
using TechSupportXPress.Models;
using TechSupportXPress.Repositories.Interfaces;

namespace TechSupportXPress.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly ApplicationDbContext _context;

        public CommentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Comment>> GetCommentsByTicketIdAsync(int ticketId)
        {
            return await _context.Comments
                .Include(c => c.CreatedBy)
                .Include(c => c.Ticket)
                .Where(c => c.TicketId == ticketId)
                .ToListAsync();
        }

        public async Task AddAsync(Comment comment)
        {
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Comment>> GetAllWithIncludesAsync()
        {
            return await _context.Comments
                .Include(c => c.CreatedBy)
                .Include(c => c.Ticket)
                .OrderByDescending(c => c.CreatedOn)
                .ToListAsync();
        }

        public async Task<List<Comment>> GetByTicketIdWithIncludesAsync(int ticketId)
        {
            return await _context.Comments
                .Where(c => c.TicketId == ticketId)
                .Include(c => c.CreatedBy)
                .Include(c => c.Ticket)
                .OrderByDescending(c => c.CreatedOn)
                .ToListAsync();
        }

        public async Task<Comment?> GetByIdWithIncludesAsync(int id)
        {
            return await _context.Comments
                .Include(c => c.CreatedBy)
                .Include(c => c.Ticket)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task UpdateAsync(Comment comment)
        {
            _context.Comments.Update(comment);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Comments.AnyAsync(c => c.Id == id);
        }

        public async Task DeleteAsync(Comment comment)
        {
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
        }

        public async Task<Comment?> GetByIdAsync(int id)
        {
            return await _context.Comments
                .Include(c => c.CreatedBy)
                .Include(c => c.Ticket)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task DeleteByTicketIdAsync(int ticketId)
        {
            var comments = await _context.Comments
                .Where(c => c.TicketId == ticketId)
                .ToListAsync();

            if (comments.Any())
            {
                _context.Comments.RemoveRange(comments);
                await _context.SaveChangesAsync();
            }
        }


    }
}
