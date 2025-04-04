using Microsoft.EntityFrameworkCore;
using TechSupportXPress.Data;
using TechSupportXPress.Models;
using TechSupportXPress.Repositories.Interfaces;
using TechSupportXPress.ViewModels;

namespace TechSupportXPress.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly ApplicationDbContext _context;

        public TicketRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<Ticket> GetAllTicketsWithIncludes()
        {
            return _context.Tickets
                .Include(t => t.CreatedBy)
                .Include(t => t.SubCategory)
                .ThenInclude(s => s.Category)
                .Include(t => t.Priority)
                .Include(t => t.Status)
                .Include(t => t.TicketComments);
        }

        public async Task<Ticket> GetTicketDetailsByIdAsync(int id)
        {
            return await _context.Tickets
                .Include(t => t.CreatedBy)
                .Include(t => t.SubCategory)
                .Include(t => t.Status)
                .Include(t => t.Priority)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<int> AddAsync(Ticket ticket)
        {
            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();
            return ticket.Id;
        }

        public async Task<Ticket> GetTicketWithSubCategoryAsync(int id)
        {
            return await _context.Tickets
                .Include(t => t.SubCategory)
                .ThenInclude(sc => sc.Category)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Ticket> GetByIdAsync(int id)
        {
            return await _context.Tickets.FindAsync(id);
        }

        public async Task UpdateAsync(Ticket ticket)
        {
            _context.Tickets.Update(ticket);
            await _context.SaveChangesAsync();
        }

        public bool TicketExists(int id)
        {
            return _context.Tickets.Any(e => e.Id == id);
        }

        public async Task<Ticket> GetWithCreatedByAsync(int id)
        {
            return await _context.Tickets
                .Include(t => t.CreatedBy)
                .Include(t => t.Status)
                .Include(t => t.Priority)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task DeleteAsync(Ticket ticket)
        {
            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
        }

        public async Task<Ticket> GetTicketForResolveAsync(int id)
        {
            return await _context.Tickets
                .Include(t => t.CreatedBy)
                .Include(t => t.SubCategory)
                .Include(t => t.Status)
                .Include(t => t.Priority)
                .Include(t => t.AssignedTo)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<List<Ticket>> GetTicketsByStatusIdsAsync(List<int> statusIds)
        {
            return await _context.Tickets
                .Include(t => t.CreatedBy)
                .Include(t => t.SubCategory)
                .Include(t => t.Priority)
                .Include(t => t.Status)
                .Include(t => t.TicketComments)
                .Where(t => statusIds.Contains(t.StatusId))
                .OrderByDescending(t => t.CreatedOn)
                .ToListAsync();
        }

        public async Task<List<Ticket>> GetAllAsync()
        {
            return await _context.Tickets.ToListAsync();
        }


    }
}
