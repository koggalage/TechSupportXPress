using Microsoft.EntityFrameworkCore;
using TechSupportXPress.Data;
using TechSupportXPress.Models;
using TechSupportXPress.Repositories.Interfaces;

namespace TechSupportXPress.Repositories
{
    public class TicketResolutionRepository : ITicketResolutionRepository
    {
        private readonly ApplicationDbContext _context;

        public TicketResolutionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<TicketResolution>> GetResolutionsByTicketIdAsync(int ticketId)
        {
            return await _context.TicketResolutions
                .Include(tr => tr.CreatedBy)
                .Include(tr => tr.Ticket)
                .Include(tr => tr.Status)
                .Where(tr => tr.TicketId == ticketId)
                .ToListAsync();
        }

        public async Task AddAsync(TicketResolution resolution)
        {
            _context.TicketResolutions.Add(resolution);
            await _context.SaveChangesAsync();
        }


    }
}
