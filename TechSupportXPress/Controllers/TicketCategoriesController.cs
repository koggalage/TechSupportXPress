using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TechSupportXPress.Data;
using TechSupportXPress.Models;

namespace TechSupportXPress.Controllers
{
    public class TicketCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TicketCategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";
            base.OnActionExecuting(context);
        }

        // GET: TicketCategories
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.TicketCategories.Include(t => t.CreatedBy).Include(t => t.DeletedBy).Include(t => t.ModifiedBy).OrderByDescending(a => a.CreatedOn);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: TicketCategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketCategory = await _context.TicketCategories
                .Include(t => t.CreatedBy)
                .Include(t => t.DeletedBy)
                .Include(t => t.ModifiedBy)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticketCategory == null)
            {
                return NotFound();
            }

            return View(ticketCategory);
        }

        // GET: TicketCategories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TicketCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TicketCategory ticketCategory)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            ticketCategory.CreatedOn = DateTime.Now;
            ticketCategory.CreatedById = userId;

                _context.Add(ticketCategory);
                await _context.SaveChangesAsync();

            var newValues = JsonConvert.SerializeObject(ticketCategory, Formatting.Indented);

            // Audit log
            var audit = new AuditTrail
            {
                Action = "Create",
                TimeStamp = DateTime.Now,
                IpAddress = ipAddress,
                UserId = userId,
                Module = "Ticket Categories",
                AffectedTable = "TicketCategories",
                OldValues = null,
                NewValues = newValues
            };

            _context.Add(audit);
            await _context.SaveChangesAsync();

            TempData["MESSAGE"] = "Ticket Category Successfully Added";

            return RedirectToAction(nameof(Index));
        }

        // GET: TicketCategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketCategory = await _context.TicketCategories.FindAsync(id);
            if (ticketCategory == null)
            {
                return NotFound();
            }
            return View(ticketCategory);
        }

        // POST: TicketCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TicketCategory ticketCategory)
        {
            if (id != ticketCategory.Id)
                return NotFound();

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

                var existing = await _context.TicketCategories
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (existing == null)
                    return NotFound();

                // Serialize old values
                var oldValues = JsonConvert.SerializeObject(existing, Formatting.Indented);

                // Update fields
                ticketCategory.CreatedById = existing.CreatedById;
                ticketCategory.CreatedOn = existing.CreatedOn;
                ticketCategory.ModifiedOn = DateTime.Now;
                ticketCategory.ModifiedById = userId;

                _context.Update(ticketCategory);
                await _context.SaveChangesAsync();

                // Serialize new values
                var newValues = JsonConvert.SerializeObject(ticketCategory, Formatting.Indented);

                // Audit log
                var audit = new AuditTrail
                {
                    Action = "Edit",
                    TimeStamp = DateTime.Now,
                    IpAddress = ipAddress,
                    UserId = userId,
                    Module = "Ticket Categories",
                    AffectedTable = "TicketCategories",
                    OldValues = oldValues,
                    NewValues = newValues
                };

                _context.Add(audit);
                await _context.SaveChangesAsync();

                TempData["MESSAGE"] = "Ticket Category Successfully Updated";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TicketCategoryExists(ticketCategory.Id))
                    return NotFound();

                throw;
            }
        }


        // GET: TicketCategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketCategory = await _context.TicketCategories
                .Include(t => t.CreatedBy)
                .Include(t => t.DeletedBy)
                .Include(t => t.ModifiedBy)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticketCategory == null)
            {
                return NotFound();
            }

            return View(ticketCategory);
        }

        // POST: TicketCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticketCategory = await _context.TicketCategories
                .Include(c => c.CreatedBy)
                .Include(c => c.ModifiedBy)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (ticketCategory == null)
                return NotFound();

            var hasSubCategories = await _context.TicketSubCategories
                .AnyAsync(sc => sc.CategoryId == id);

            if (hasSubCategories)
            {
                TempData["Error"] = "Cannot delete this category as it has related sub-categories.";
                return RedirectToAction(nameof(Index));
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            // Serialize old state before deletion
            var oldValues = JsonConvert.SerializeObject(ticketCategory, Formatting.Indented);

            _context.TicketCategories.Remove(ticketCategory);
            await _context.SaveChangesAsync();

            var audit = new AuditTrail
            {
                Action = "Delete",
                TimeStamp = DateTime.Now,
                IpAddress = ipAddress,
                UserId = userId,
                Module = "Ticket Categories",
                AffectedTable = "TicketCategories",
                OldValues = oldValues,
                NewValues = null
            };

            _context.AuditTrails.Add(audit);
            await _context.SaveChangesAsync();

            TempData["MESSAGE"] = "Category deleted successfully";
            return RedirectToAction(nameof(Index));
        }



        private bool TicketCategoryExists(int id)
        {
            return _context.TicketCategories.Any(e => e.Id == id);
        }
    }
}
