using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TechSupportXPress.Data;
using TechSupportXPress.Models;
using TechSupportXPress.ViewModels;

namespace TechSupportXPress.Controllers
{
    public class TicketSubCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TicketSubCategoriesController(ApplicationDbContext context)
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

        // GET: TicketSubCategories
        public async Task<IActionResult> Index(int id, TicketSubCategoriesVM vm)
        {
            vm.TicketSubCategories = _context.TicketSubCategories
                .Include(t => t.Category)
                .Include(t => t.CreatedBy)
                .Include(t => t.ModifiedBy)
                .Where(x => x.CategoryId == id)
                .OrderByDescending(a => a.CreatedOn)
                .ToList();

            vm.CategoryId = id;

            return View(vm);
        }

        public async Task<IActionResult> SubCategories(TicketSubCategoriesVM vm)
        {
            vm.TicketSubCategories = await _context.TicketSubCategories
                .Include(t => t.Category)
                .Include(t => t.CreatedBy)
                .Include(t => t.ModifiedBy)
                .ToListAsync();

          //  ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "FullName");

          //  ViewData["CategoryId"] = new SelectList(_context.TicketCategories, "Id", "Name");


            return View(vm);
        }

        // GET: TicketSubCategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketSubCategory = await _context.TicketSubCategories
                .Include(t => t.Category)
                .Include(t => t.CreatedBy)
                .Include(t => t.DeletedBy)
                .Include(t => t.ModifiedBy)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticketSubCategory == null)
            {
                return NotFound();
            }

            return View(ticketSubCategory);
        }

        // GET: TicketSubCategories/Create
        public IActionResult Create(int id)
        {
            TicketSubCategory category = new();
            category.CategoryId = id;

            return View(category);
        }

        // POST: TicketSubCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int id, TicketSubCategory ticketSubCategory)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            ticketSubCategory.CreatedById = userId;
            ticketSubCategory.CreatedOn = DateTime.Now;
            ticketSubCategory.Id = 0; // Resetting for EF to auto-generate
            ticketSubCategory.CategoryId = id;

            _context.Add(ticketSubCategory);
            await _context.SaveChangesAsync();

            var newValues = JsonConvert.SerializeObject(ticketSubCategory, Formatting.Indented);

            var audit = new AuditTrail
            {
                Action = "Create",
                TimeStamp = DateTime.Now,
                IpAddress = ipAddress,
                UserId = userId,
                Module = "Ticket Sub-Categories",
                AffectedTable = "TicketSubCategories",
                NewValues = newValues
            };

            _context.Add(audit);
            await _context.SaveChangesAsync();

            TempData["MESSAGE"] = "Ticket Sub-Category Details successfully Created";
            return RedirectToAction("Index", new { id = id });
        }


        // GET: TicketSubCategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketSubCategory = await _context.TicketSubCategories.FindAsync(id);
            if (ticketSubCategory == null)
            {
                return NotFound();
            }
            return View(ticketSubCategory);
        }

        // POST: TicketSubCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TicketSubCategory ticketSubCategory)
        {
            if (id != ticketSubCategory.Id)
                return NotFound();

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

                var oldSubCategory = await _context.TicketSubCategories
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (oldSubCategory == null)
                    return NotFound();

                // Set audit fields
                ticketSubCategory.ModifiedById = userId;
                ticketSubCategory.ModifiedOn = DateTime.Now;

                _context.Update(ticketSubCategory);
                await _context.SaveChangesAsync();

                var oldValues = JsonConvert.SerializeObject(oldSubCategory, Formatting.Indented);
                var newValues = JsonConvert.SerializeObject(ticketSubCategory, Formatting.Indented);

                var audit = new AuditTrail
                {
                    Action = "Edit",
                    TimeStamp = DateTime.Now,
                    IpAddress = ipAddress,
                    UserId = userId,
                    Module = "Ticket Sub-Categories",
                    AffectedTable = "TicketSubCategories",
                    OldValues = oldValues,
                    NewValues = newValues
                };

                _context.Add(audit);
                await _context.SaveChangesAsync();

                TempData["MESSAGE"] = "Ticket Sub-Category Details successfully Updated";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TicketSubCategoryExists(ticketSubCategory.Id))
                    return NotFound();

                throw;
            }

            return RedirectToAction("Index", new { id = ticketSubCategory.CategoryId });
        }


        // GET: TicketSubCategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketSubCategory = await _context.TicketSubCategories
                .Include(t => t.Category)
                .Include(t => t.CreatedBy)
                .Include(t => t.DeletedBy)
                .Include(t => t.ModifiedBy)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticketSubCategory == null)
            {
                return NotFound();
            }

            return View(ticketSubCategory);
        }

        // POST: TicketSubCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticketSubCategory = await _context.TicketSubCategories
                .Include(t => t.CreatedBy)
                .Include(t => t.ModifiedBy)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticketSubCategory == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var oldValues = JsonConvert.SerializeObject(ticketSubCategory, Formatting.Indented);

            _context.TicketSubCategories.Remove(ticketSubCategory);
            await _context.SaveChangesAsync();

            var audit = new AuditTrail
            {
                Action = "Delete",
                TimeStamp = DateTime.Now,
                IpAddress = ipAddress,
                UserId = userId,
                Module = "Ticket Sub-Categories",
                AffectedTable = "TicketSubCategories",
                OldValues = oldValues,
                NewValues = null
            };

            _context.Add(audit);
            await _context.SaveChangesAsync();

            TempData["MESSAGE"] = "Ticket Sub-Category successfully deleted";
            return RedirectToAction("Index", new { id = ticketSubCategory.CategoryId });
        }


        private bool TicketSubCategoryExists(int id)
        {
            return _context.TicketSubCategories.Any(e => e.Id == id);
        }
    }
}
