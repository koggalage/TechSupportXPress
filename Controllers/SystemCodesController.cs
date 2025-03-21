using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechSupportXPress.Data;
using TechSupportXPress.Models;
using TechSupportXPress.ViewModels;

namespace TechSupportXPress.Controllers
{
    public class SystemCodesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SystemCodesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SystemCodes
        public async Task<IActionResult> Index(SystemCodeViewModel vm)
        {
            var systemcodes = _context
                .SystemCodes
                .Include(x => x.CreatedBy)
                .AsQueryable();

            if (vm != null)
            {
                if (!string.IsNullOrEmpty(vm.Code))
                {
                    systemcodes = systemcodes.Where(x => x.Code.Contains(vm.Code));
                }
                if (!string.IsNullOrEmpty(vm.CreatedById))
                {
                    systemcodes = systemcodes.Where(x => x.CreatedById == vm.CreatedById);
                }
                if (!string.IsNullOrEmpty(vm.Description))
                {
                    systemcodes = systemcodes.Where(x => x.Description == vm.Description);
                }
            }


            vm.SystemCodes = await systemcodes.ToListAsync();

            ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "FullName");

            return View(vm);
        }

        // GET: SystemCodes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemCode = await _context.SystemCodes
                .Include(s => s.CreatedBy)
                .Include(s => s.DeletedBy)
                .Include(s => s.ModifiedBy)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (systemCode == null)
            {
                return NotFound();
            }

            return View(systemCode);
        }

        // GET: SystemCodes/Create
        public IActionResult Create()
        {
            ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["DeletedById"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["ModifiedById"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: SystemCodes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SystemCode systemCode)
        {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                systemCode.CreatedOn = DateTime.Now;
                systemCode.CreatedById = userId;

                _context.Add(systemCode);
                await _context.SaveChangesAsync();

                //Audit Log
                var activity = new AuditTrail
                {
                    Action = "Create",
                    TimeStamp = DateTime.Now,
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                    UserId = userId,
                    Module = "System Codes",
                    AffectedTable = "SystemCodes"
                };

                return RedirectToAction(nameof(Index));
        }

        // GET: SystemCodes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemCode = await _context.SystemCodes.FindAsync(id);
            if (systemCode == null)
            {
                return NotFound();
            }
            ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "Id", systemCode.CreatedById);
            ViewData["DeletedById"] = new SelectList(_context.Users, "Id", "Id", systemCode.DeletedById);
            ViewData["ModifiedById"] = new SelectList(_context.Users, "Id", "Id", systemCode.ModifiedById);
            return View(systemCode);
        }

        // POST: SystemCodes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SystemCode systemCode)
        {
            if (id != systemCode.Id)
            {
                return NotFound();
            }

                try
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    systemCode.ModifiedOn = DateTime.Now;
                    systemCode.ModifiedById = userId;

                    _context.Update(systemCode);
                    await _context.SaveChangesAsync();

                    //Audit Log
                    var activity = new AuditTrail
                    {
                        Action = "Edit",
                        TimeStamp = DateTime.Now,
                        IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                        UserId = userId,
                        Module = "System Codes",
                        AffectedTable = "SystemCodes"
                    };
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SystemCodeExists(systemCode.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
        }

        // GET: SystemCodes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemCode = await _context.SystemCodes
                .Include(s => s.CreatedBy)
                .Include(s => s.DeletedBy)
                .Include(s => s.ModifiedBy)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (systemCode == null)
            {
                return NotFound();
            }

            return View(systemCode);
        }

        // POST: SystemCodes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var systemCode = await _context.SystemCodes.FindAsync(id);
            if (systemCode != null)
            {
                _context.SystemCodes.Remove(systemCode);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SystemCodeExists(int id)
        {
            return _context.SystemCodes.Any(e => e.Id == id);
        }
    }
}
