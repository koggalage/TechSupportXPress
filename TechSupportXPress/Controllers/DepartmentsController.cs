﻿using System;
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

namespace TechSupportXPress.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DepartmentsController(ApplicationDbContext context)
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

        // GET: Departments
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Departments
                .Include(d => d.CreatedBy)
                .Include(d => d.DeletedBy)
                .Include(d => d.ModifiedBy)
                .OrderByDescending(a => a.CreatedOn);

            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Departments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .Include(d => d.CreatedBy)
                .Include(d => d.DeletedBy)
                .Include(d => d.ModifiedBy)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        // GET: Departments/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Departments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Department department)
        {
            var existing = await _context.Departments
                .AnyAsync(d => d.Code.ToLower() == department.Code.ToLower());

            if (existing)
            {
                ModelState.AddModelError("Code", "Department code already exists.");
                return View(department);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            department.CreatedOn = DateTime.Now;
            department.CreatedById = userId;

            // Capture new values before save
            var newValues = JsonConvert.SerializeObject(department, Formatting.Indented);

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            // Audit log
            var activity = new AuditTrail
            {
                Action = "Create",
                TimeStamp = DateTime.Now,
                IpAddress = ipAddress,
                UserId = userId,
                Module = "Departments",
                AffectedTable = "Departments",
                NewValues = newValues
            };

            _context.AuditTrails.Add(activity);
            await _context.SaveChangesAsync();

            TempData["MESSAGE"] = "Department successfully created.";
            return RedirectToAction(nameof(Index));
        }



        // GET: Departments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        // POST: Departments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Department department)
        {
            if (id != department.Id)
                return NotFound();

            var exists = await _context.Departments
                .AnyAsync(d => d.Code.ToLower() == department.Code.ToLower() && d.Id != department.Id);

            if (exists)
            {
                ModelState.AddModelError("Code", "Department code already exists.");
                return View(department);
            }

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

                var original = await _context.Departments
                    .AsNoTracking()
                    .FirstOrDefaultAsync(d => d.Id == id);

                if (original == null)
                    return NotFound();

                var oldValues = JsonConvert.SerializeObject(original, Formatting.Indented);

                department.ModifiedOn = DateTime.Now;
                department.ModifiedById = userId;

                _context.Update(department);
                await _context.SaveChangesAsync();

                var newValues = JsonConvert.SerializeObject(department, Formatting.Indented);

                var activity = new AuditTrail
                {
                    Action = "Edit",
                    TimeStamp = DateTime.Now,
                    IpAddress = ipAddress,
                    UserId = userId,
                    Module = "Departments",
                    AffectedTable = "Departments",
                    OldValues = oldValues,
                    NewValues = newValues
                };

                _context.Add(activity);
                await _context.SaveChangesAsync();

                TempData["MESSAGE"] = "Department successfully updated.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DepartmentExists(department.Id))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToAction(nameof(Index));
        }



        // GET: Departments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .Include(d => d.CreatedBy)
                .Include(d => d.DeletedBy)
                .Include(d => d.ModifiedBy)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        // POST: Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var department = await _context.Departments
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id);

            if (department == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            var oldValues = JsonConvert.SerializeObject(department, Formatting.Indented);

            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();

            var activity = new AuditTrail
            {
                Action = "Delete",
                TimeStamp = DateTime.Now,
                IpAddress = ipAddress,
                UserId = userId,
                Module = "Departments",
                AffectedTable = "Departments",
                OldValues = oldValues,
                NewValues = null
            };

            _context.AuditTrails.Add(activity);
            await _context.SaveChangesAsync();

            TempData["MESSAGE"] = "Department deleted successfully.";
            return RedirectToAction(nameof(Index));
        }


        private bool DepartmentExists(int id)
        {
            return _context.Departments.Any(e => e.Id == id);
        }
    }
}
