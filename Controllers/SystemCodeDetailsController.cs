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
    public class SystemCodeDetailsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SystemCodeDetailsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SystemCodeDetails
        public async Task<IActionResult> Index(SystemCodeDetailViewModel vm)
        {
            var systemCodeDetails = _context
                .SystemCodeDetails
                .Include(s => s.SystemCode)
                .Include(s => s.CreatedBy)
                .AsQueryable();

            if (vm != null)
            {
                if (!string.IsNullOrEmpty(vm.Code))
                {
                    systemCodeDetails = systemCodeDetails.Where(x => x.Code.Contains(vm.Code));
                }
                if (!string.IsNullOrEmpty(vm.CreatedById))
                {
                    systemCodeDetails = systemCodeDetails.Where(x => x.CreatedById == vm.CreatedById);
                }
                if (!string.IsNullOrEmpty(vm.Description))
                {
                    systemCodeDetails = systemCodeDetails.Where(x => x.Description == vm.Description);
                }

                if (vm.SystemCodeId > 0)
                {
                    systemCodeDetails = systemCodeDetails.Where(x => x.SystemCodeId == vm.SystemCodeId);
                }
            }


            vm.SystemCodeDetails = await systemCodeDetails.ToListAsync();

            ViewData["SystemCodeId"] = new SelectList(_context.SystemCodes, "Id", "Description");
            ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "FullName");
            return View(vm);
        }

        // GET: SystemCodeDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemCodeDetail = await _context.SystemCodeDetails
                .Include(s => s.CreatedBy)
                .Include(s => s.DeletedBy)
                .Include(s => s.ModifiedBy)
                .Include(s => s.SystemCode)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (systemCodeDetail == null)
            {
                return NotFound();
            }

            return View(systemCodeDetail);
        }

        // GET: SystemCodeDetails/Create
        public IActionResult Create()
        {
            ViewData["SystemCodeId"] = new SelectList(_context.SystemCodes, "Id", "Id");
            return View();
        }

        // POST: SystemCodeDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SystemCodeDetail systemCodeDetail)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            systemCodeDetail.CreatedOn = DateTime.Now;
            systemCodeDetail.CreatedById = userId;

            _context.Add(systemCodeDetail);
                await _context.SaveChangesAsync();

            //Audit Log
            var activity = new AuditTrail
            {
                Action = "Create",
                TimeStamp = DateTime.Now,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                UserId = userId,
                Module = "System Code Details",
                AffectedTable = "SystemCodeDetails"
            };

            return RedirectToAction(nameof(Index));
        }

        // GET: SystemCodeDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemCodeDetail = await _context.SystemCodeDetails.FindAsync(id);
            if (systemCodeDetail == null)
            {
                return NotFound();
            }

            ViewData["SystemCodeId"] = new SelectList(_context.SystemCodes, "Id", "Description", systemCodeDetail.SystemCodeId);
            return View(systemCodeDetail);
        }

        // POST: SystemCodeDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SystemCodeDetail systemCodeDetail)
        {
            if (id != systemCodeDetail.Id)
            {
                return NotFound();
            }

                try
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    systemCodeDetail.ModifiedOn = DateTime.Now;
                    systemCodeDetail.ModifiedById = userId;

                    _context.Update(systemCodeDetail);
                    await _context.SaveChangesAsync();

                    //Audit Log
                    var activity = new AuditTrail
                    {
                        Action = "Edit",
                        TimeStamp = DateTime.Now,
                        IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                        UserId = userId,
                        Module = "System Code Details",
                        AffectedTable = "SystemCodeDetails"
                    };
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SystemCodeDetailExists(systemCodeDetail.Id))
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

        // GET: SystemCodeDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemCodeDetail = await _context.SystemCodeDetails
                .Include(s => s.CreatedBy)
                .Include(s => s.DeletedBy)
                .Include(s => s.ModifiedBy)
                .Include(s => s.SystemCode)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (systemCodeDetail == null)
            {
                return NotFound();
            }

            return View(systemCodeDetail);
        }

        // POST: SystemCodeDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var systemCodeDetail = await _context.SystemCodeDetails.FindAsync(id);
            if (systemCodeDetail != null)
            {
                _context.SystemCodeDetails.Remove(systemCodeDetail);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SystemCodeDetailExists(int id)
        {
            return _context.SystemCodeDetails.Any(e => e.Id == id);
        }
    }
}
