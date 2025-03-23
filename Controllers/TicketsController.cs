using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Claims;
using System.Threading.Tasks;
using Humanizer.Localisation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using TechSupportXPress.Data;
using TechSupportXPress.Models;
using TechSupportXPress.ViewModels;
using TechSupportXPress.Resources;
using Constants = TechSupportXPress.Resources.Constants;
using Microsoft.AspNetCore.Identity;

namespace TechSupportXPress.Controllers
{
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public TicketsController(ApplicationDbContext context, IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _configuration = configuration;
            _userManager = userManager;
        }

        // GET: Tickets
        //public async Task<IActionResult> Index(TicketViewModel vm)
        //{

        //    var alltickets = _context.Tickets
        //        .Include(t => t.CreatedBy)
        //        .Include(t => t.SubCategory)
        //        .Include(t => t.Priority)
        //        .Include(t => t.Status)
        //        .Include(t => t.TicketComments)
        //        .OrderBy(x => x.CreatedOn)
        //        .AsQueryable();

        //    if (vm != null)
        //    {
        //        if (!string.IsNullOrEmpty(vm.Title))
        //        {
        //            alltickets = alltickets.Where(x => x.Title.Contains(vm.Title));
        //        }

        //        if (!string.IsNullOrEmpty(vm.CreatedById))
        //        {
        //            alltickets = alltickets.Where(x => x.CreatedById == vm.CreatedById);
        //        }

        //        if (vm.StatusId > 0)
        //        {
        //            alltickets = alltickets.Where(x => x.StatusId == vm.StatusId);
        //        }

        //        if (vm.PriorityId > 0)
        //        {
        //            alltickets = alltickets.Where(x => x.PriorityId == vm.PriorityId);
        //        }

        //        if (vm.CategoryId > 0)
        //        {
        //            alltickets = alltickets.Where(x => x.SubCategory.CategoryId == vm.CategoryId);
        //        }
        //    }

        //    vm.Tickets = await alltickets.ToListAsync();





        //    ViewData["PriorityId"] = new SelectList(_context.SystemCodeDetails
        //        .Include(x => x.SystemCode)
        //        .Where(x => x.SystemCode.Code == "Priority"), "Id", "Description");
        //    ViewData["CategoryId"] = new SelectList(_context.TicketCategories, "Id", "Name");
        //    ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "FullName");
        //    ViewData["StatusId"] = new SelectList(_context.SystemCodeDetails
        //        .Include(x => x.SystemCode)
        //        .Where(x => x.SystemCode.Code == "ResolutionStatus"), "Id", "Description");

        //    return View(vm);
        //}

        public async Task<IActionResult> Index(TicketViewModel vm)
        {
            var currentUserId = _userManager.GetUserId(User);
            var currentUser = await _userManager.GetUserAsync(User);
            var userRoles = await _userManager.GetRolesAsync(currentUser);

            var alltickets = _context.Tickets
                .Include(t => t.CreatedBy)
                .Include(t => t.SubCategory)
                .Include(t => t.Priority)
                .Include(t => t.Status)
                .Include(t => t.TicketComments)
                .OrderBy(x => x.CreatedOn)
                .AsQueryable();

            // 🔐 Apply role-based access control
            if (userRoles.Contains("ADMIN"))
            {
                // No filter, show all
            }
            else if (userRoles.Contains("SUPPORT"))
            {
                alltickets = alltickets.Where(t => t.AssignedToId == currentUserId);
            }
            else if (userRoles.Contains("USER"))
            {
                alltickets = alltickets.Where(t => t.CreatedById == currentUserId);
            }

            // 🔍 Apply filter parameters
            if (vm != null)
            {
                if (!string.IsNullOrEmpty(vm.Title))
                {
                    alltickets = alltickets.Where(x => x.Title.Contains(vm.Title));
                }

                if (!string.IsNullOrEmpty(vm.CreatedById))
                {
                    alltickets = alltickets.Where(x => x.CreatedById == vm.CreatedById);
                }

                if (vm.StatusId > 0)
                {
                    alltickets = alltickets.Where(x => x.StatusId == vm.StatusId);
                }

                if (vm.PriorityId > 0)
                {
                    alltickets = alltickets.Where(x => x.PriorityId == vm.PriorityId);
                }

                if (vm.CategoryId > 0)
                {
                    alltickets = alltickets.Where(x => x.SubCategory.CategoryId == vm.CategoryId);
                }
            }

            vm.Tickets = await alltickets.ToListAsync();

            ViewData["PriorityId"] = new SelectList(_context.SystemCodeDetails
                .Include(x => x.SystemCode)
                .Where(x => x.SystemCode.Code == "Priority"), "Id", "Description");

            ViewData["CategoryId"] = new SelectList(_context.TicketCategories, "Id", "Name");

            ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "FullName");

            ViewData["StatusId"] = new SelectList(_context.SystemCodeDetails
                .Include(x => x.SystemCode)
                .Where(x => x.SystemCode.Code == "ResolutionStatus"), "Id", "Description");

            return View(vm);
        }


        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(int? id, TicketViewModel vm)
        {
            if (id == null)
            {
                return NotFound();
            }

            vm.TicketDetails = await _context.Tickets
               .Include(t => t.CreatedBy)
               .Include(t => t.SubCategory)
               .Include(t => t.Status)
               .Include(t => t.Priority)
               .FirstOrDefaultAsync(m => m.Id == id);



            if (vm.TicketDetails == null)
            {
                return NotFound();
            }

            vm.TicketComments = await _context.Comments
               .Include(t => t.CreatedBy)
               .Include(t => t.Ticket)
               .Where(t => t.TicketId == id)
               .ToListAsync();

            vm.TicketResolutions = await _context.TicketResolutions
               .Include(t => t.CreatedBy)
               .Include(t => t.Ticket)
               .Include(t => t.Status)
               .Where(t => t.TicketId == id)
               .ToListAsync();

            return View(vm);
        }

        // GET: Tickets/Create
        public IActionResult Create()
        {
            ViewData["PriorityId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCode).Where(x => x.SystemCode.Code == "Priority"), "Id", "Description");
            ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "FullName");
            ViewData["CategoryId"] = new SelectList(_context.TicketCategories, "Id", "Name");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TicketViewModel ticketvm, IFormFile attachment)
        {
            if (attachment != null && attachment.Length > 0)
            {
                // Ensure the "uploads" folder exists inside wwwroot
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // Clean and create unique filename
                var filename = "Ticket_Attachment_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Path.GetFileName(attachment.FileName);
                var filePath = Path.Combine(uploadsFolder, filename);

                using var stream = new FileStream(filePath, FileMode.Create);
                await attachment.CopyToAsync(stream);

                ticketvm.Attachment = filename;
            }

            var pendingStatus = await _context
                .SystemCodeDetails
                .Include(x => x.SystemCode)
                .FirstOrDefaultAsync(x => x.SystemCode.Code == "ResolutionStatus" && x.Description == "Pending");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            Ticket ticket = new()
            {
                Title = ticketvm.Title,
                Description = ticketvm.Description,
                StatusId = pendingStatus.Id,
                PriorityId = ticketvm.PriorityId,
                SubCategoryId = ticketvm.SubCategoryId,
                Attachment = ticketvm.Attachment,
                CreatedById = userId,
                CreatedOn = DateTime.Now
            };

            _context.Add(ticket);
            await _context.SaveChangesAsync();

            // 📝 Audit Trail
            var activity = new AuditTrail
            {
                Action = "Create",
                TimeStamp = DateTime.Now,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                UserId = userId,
                Module = "Tickets",
                AffectedTable = "Tickets"
            };

            _context.Add(activity);
            await _context.SaveChangesAsync();

            TempData["MESSAGE"] = "Ticket Details successfully Created";

            return RedirectToAction(nameof(Index));
        }


        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var ticket = await _context.Tickets
                .Include(t => t.SubCategory)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null)
                return NotFound();

            var viewModel = new TicketViewModel
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Description = ticket.Description,
                PriorityId = ticket.PriorityId,
                CategoryId = ticket.SubCategory.CategoryId,
                SubCategoryId = ticket.SubCategoryId,
                Attachment = ticket.Attachment
            };

            ViewData["PriorityId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCode).Where(x => x.SystemCode.Code == "Priority"), "Id", "Description", ticket.PriorityId);
            ViewData["CategoryId"] = new SelectList(_context.TicketCategories, "Id", "Name", viewModel.CategoryId);
            ViewData["SubCategoryId"] = new SelectList(_context.TicketSubCategories.Where(s => s.CategoryId == viewModel.CategoryId), "Id", "Name", viewModel.SubCategoryId);

            return View(viewModel);
        }


        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TicketViewModel model, IFormFile attachment)
        {
            if (id != model.Id)
                return NotFound();

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
                return NotFound();

            // Handle file upload
            if (attachment != null && attachment.Length > 0)
            {
                // Ensure the folder exists
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // Save with timestamp to avoid conflicts
                var fileName = "Ticket_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Path.GetFileName(attachment.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await attachment.CopyToAsync(stream);

                ticket.Attachment = fileName;
            }

            // Update ticket properties
            ticket.Title = model.Title;
            ticket.Description = model.Description;
            ticket.PriorityId = model.PriorityId;
            ticket.SubCategoryId = model.SubCategoryId;
            ticket.ModifiedOn = DateTime.Now;
            ticket.ModifiedById = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                _context.Update(ticket);
                await _context.SaveChangesAsync();

                TempData["MESSAGE"] = "Ticket updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TicketExists(ticket.Id))
                    return NotFound();

                throw;
            }
        }



        // GET: Tickets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(t => t.CreatedBy)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket != null)
            {
                _context.Tickets.Remove(ticket);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(int id, TicketViewModel vm)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                Comment newcomment = new();
                newcomment.TicketId = id;
                newcomment.CreatedOn = DateTime.Now;
                newcomment.CreatedById = userId;
                newcomment.Description = vm.CommentDescription;
                _context.Add(newcomment);
                await _context.SaveChangesAsync();

                //Audit Log
                var activity = new AuditTrail
                {
                    Action = "Create",
                    TimeStamp = DateTime.Now,
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                    UserId = userId,
                    Module = "Comments",
                    AffectedTable = "Comments"
                };

                _context.Add(activity);
                await _context.SaveChangesAsync();


                TempData["MESSAGE"] = "Comments Details successfully Created";

                return RedirectToAction("Details", new { id = id });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Comment Details could not be Created" + ex.Message;
                return View(vm);
            }
        }

        public async Task<IActionResult> Resolve(int? id, TicketViewModel vm)
        {
            if (id == null)
            {
                return NotFound();
            }

            vm.TicketDetails = await _context.Tickets
               .Include(t => t.CreatedBy)
               .Include(t => t.SubCategory)
               .Include(t => t.Status)
               .Include(t => t.Priority)
               .Include(t => t.AssignedTo)
               .FirstOrDefaultAsync(m => m.Id == id);

            vm.TicketComments = await _context.Comments
                .Include(t => t.CreatedBy)
                .Include(t => t.Ticket)
                .Where(t => t.TicketId == id)
                .ToListAsync();

            vm.TicketResolutions = await _context.TicketResolutions
               .Include(t => t.CreatedBy)
               .Include(t => t.Ticket)
               .Include(t => t.Status)
               .Where(t => t.TicketId == id)
               .ToListAsync();

            string nextStatus = vm.TicketDetails.Status.Code switch
            {
                "Pending" => "Assigned",
                "Assigned" => "Resolved",
                "Resolved" => "Closed",
                "Closed" => "ReOpened",
                "ReOpened" => "Assigned",
                _ => "N/A"
            };

            var nextStatusObj = await _context.SystemCodeDetails
     .Include(x => x.SystemCode)
     .FirstOrDefaultAsync(x => x.SystemCode.Code == "ResolutionStatus" && x.Description == nextStatus);

            ViewBag.NextStatus = nextStatusObj?.Description;
            ViewBag.NextStatusId = nextStatusObj?.Id;


            if (vm.TicketDetails == null)
            {
                return NotFound();
            }


            //ViewData["StatusId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCode).Where(x => x.SystemCode.Code == "ResolutionStatus"), "Id", "Description");

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> ResolvedConfirmed(int id, TicketViewModel vm)
        {
            //Logged In User
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            TicketResolution resolution = new();
            resolution.TicketId = id;
            resolution.StatusId = vm.StatusId;
            resolution.CreatedOn = DateTime.Now;
            resolution.CreatedById = userId;
            resolution.Description = vm.CommentDescription;
            _context.Add(resolution);

            var ticket = await _context.Tickets
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
            ticket.StatusId = vm.StatusId;
            _context.Update(ticket);

            await _context.SaveChangesAsync();

            //Audit Log
            var activity = new AuditTrail
            {
                Action = "Update",
                TimeStamp = DateTime.Now,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                UserId = userId,
                Module = "Ticket Resolution",
                AffectedTable = "TicketResolution"
            };

            _context.Add(activity);
            await _context.SaveChangesAsync();

            TempData["MESSAGE"] = $"Ticket status changed to {vm.NextStatus} successfully Created";

            //return RedirectToAction("Resolve", new { id = id });
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Close(int? id, TicketViewModel vm)
        {
            if (id == null)
            {
                return NotFound();
            }

            vm.TicketDetails = await _context.Tickets
               .Include(t => t.CreatedBy)
               .Include(t => t.SubCategory)
               .Include(t => t.Status)
               .Include(t => t.Priority)
               .Include(t => t.AssignedTo)
               .FirstOrDefaultAsync(m => m.Id == id);

            vm.TicketComments = await _context.Comments
                .Include(t => t.CreatedBy)
                .Include(t => t.Ticket)
                .Where(t => t.TicketId == id)
                .ToListAsync();

            vm.TicketResolutions = await _context.TicketResolutions
               .Include(t => t.CreatedBy)
               .Include(t => t.Ticket)
               .Include(t => t.Status)
               .Where(t => t.TicketId == id)
               .ToListAsync();


            if (vm.TicketDetails == null)
            {
                return NotFound();
            }


            ViewData["StatusId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCode).Where(x => x.SystemCode.Code == "ResolutionStatus"), "Id", "Description");

            return View(vm);
        }

        public async Task<IActionResult> ClosedConfirmed(int id, TicketViewModel vm)
        {

            var closedstatus = await _context.SystemCodeDetails
                .Include(x => x.SystemCode)
                .Where(x => x.SystemCode.Code == "ResolutionStatus" && x.Code == "Closed")
                .FirstOrDefaultAsync();

            //Logged In User
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            TicketResolution resolution = new();
            resolution.TicketId = id;
            resolution.StatusId = closedstatus.Id;
            resolution.CreatedOn = DateTime.Now;
            resolution.CreatedById = userId;
            resolution.Description = "Ticket Closed";
            _context.Add(resolution);

            var ticket = await _context.Tickets
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            ticket.StatusId = closedstatus.Id;
            _context.Update(ticket);

            await _context.SaveChangesAsync();

            //Audit Log
            var activity = new AuditTrail
            {
                Action = "Closed",
                TimeStamp = DateTime.Now,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                UserId = userId,
                Module = "Ticket Resolution",
                AffectedTable = "TicketResolution"
            };

            _context.Add(activity);
            await _context.SaveChangesAsync();

            TempData["MESSAGE"] = "Ticket Closed successfully";

            return RedirectToAction("Resolve", new { id = id });
        }

        public async Task<IActionResult> ReOpen(int? id, TicketViewModel vm)
        {
            if (id == null)
            {
                return NotFound();
            }

            vm.TicketDetails = await _context.Tickets
               .Include(t => t.CreatedBy)
               .Include(t => t.SubCategory)
               .Include(t => t.Status)
               .Include(t => t.Priority)
               .Include(t => t.AssignedTo)
               .FirstOrDefaultAsync(m => m.Id == id);

            vm.TicketComments = await _context.Comments
                .Include(t => t.CreatedBy)
                .Include(t => t.Ticket)
                .Where(t => t.TicketId == id)
                .ToListAsync();

            vm.TicketResolutions = await _context.TicketResolutions
               .Include(t => t.CreatedBy)
               .Include(t => t.Ticket)
               .Include(t => t.Status)
               .Where(t => t.TicketId == id)
               .ToListAsync();


            if (vm.TicketDetails == null)
            {
                return NotFound();
            }


            ViewData["StatusId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCode).Where(x => x.SystemCode.Code == "ResolutionStatus"), "Id", "Description");

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> ReOpenConfirmed(int id, TicketViewModel vm)
        {

            var closedstatus = await _context.SystemCodeDetails
                .Include(x => x.SystemCode)
                .Where(x => x.SystemCode.Code == "ResolutionStatus" && x.Code == "ReOpened")
                .FirstOrDefaultAsync();

            //Logged In User
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            TicketResolution resolution = new();
            resolution.TicketId = id;
            resolution.StatusId = closedstatus.Id;
            resolution.CreatedOn = DateTime.Now;
            resolution.CreatedById = userId;
            resolution.Description = "Ticket Re-Opened";
            _context.Add(resolution);

            var ticket = await _context.Tickets
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            ticket.StatusId = closedstatus.Id;
            _context.Update(ticket);

            await _context.SaveChangesAsync();

            //Audit Log
            var activity = new AuditTrail
            {
                Action = "Re-Open",
                TimeStamp = DateTime.Now,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                UserId = userId,
                Module = "Ticket Resolution",
                AffectedTable = "TicketResolution"
            };

            _context.Add(activity);
            await _context.SaveChangesAsync();



            TempData["MESSAGE"] = "Ticket Re-Opened successfully";

            return RedirectToAction("Resolve", new { id = id });
        }

        public async Task<IActionResult> TicketAssignment(int? id, TicketViewModel vm)
        {
            if (id == null)
            {
                return NotFound();
            }

            vm.TicketDetails = await _context.Tickets
               .Include(t => t.CreatedBy)
               .Include(t => t.SubCategory)
               .Include(t => t.Status)
               .Include(t => t.Priority)
               .Include(t => t.AssignedTo)
               .FirstOrDefaultAsync(m => m.Id == id);

            if (vm.TicketDetails == null)
            {
                return NotFound();
            }

            vm.TicketComments = await _context.Comments
                .Include(t => t.CreatedBy)
                .Include(t => t.Ticket)
                .Where(t => t.TicketId == id)
                .ToListAsync();

            vm.TicketResolutions = await _context.TicketResolutions
               .Include(t => t.CreatedBy)
               .Include(t => t.Ticket)
               .Include(t => t.Status)
               .Where(t => t.TicketId == id)
               .ToListAsync();

            ViewData["StatusId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCode).Where(x => x.SystemCode.Code == "ResolutionStatus"), "Id", "Description");

            // Get the SUPPORT role ID
            var supportRoleId = await _context.Roles
                .Where(r => r.Name == "SUPPORT")
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            // Get users who are assigned to the SUPPORT role
            var supportUsers = await _context.Users
                .Where(u => _context.UserRoles
                    .Any(ur => ur.UserId == u.Id && ur.RoleId == supportRoleId))
                .ToListAsync();

            ViewData["UserId"] = new SelectList(supportUsers, "Id", "FullName");

            // ViewData["UserId"] = new SelectList(_context.Users, "Id", "FullName");

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> AssignedConfirmed(int id, TicketViewModel vm)
        {

            try
            {
                var reassignedstatus = await _context.SystemCodeDetails
                    .Include(x => x.SystemCode)
                    .Where(x => x.SystemCode.Code == Constants.SYSTEM_CODE_RESOLUTION_STATUS &&
                                x.Code == Constants.RESOLUTION_STATUS_ASSIGNED)
                    .FirstOrDefaultAsync();

                //Logged In User
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                TicketResolution resolution = new();
                resolution.TicketId = id;
                resolution.StatusId = reassignedstatus.Id;
                resolution.CreatedOn = DateTime.Now;
                resolution.CreatedById = userId;
                resolution.Description = "Ticket Assigned";
                _context.Add(resolution);

                var ticket = await _context.Tickets
                    .Where(x => x.Id == id)
                    .FirstOrDefaultAsync();

                ticket.StatusId = reassignedstatus.Id;
                ticket.AssignedToId = vm.AssignedToId;
                ticket.AssignedOn = DateTime.Now;
                _context.Update(ticket);

                await _context.SaveChangesAsync();

                //Audit Log
                var activity = new AuditTrail
                {
                    Action = Constants.RESOLUTION_STATUS_ASSIGNED,
                    TimeStamp = DateTime.Now,
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                    UserId = userId,
                    Module = "Ticket",
                    AffectedTable = "Ticket"
                };

                _context.Add(activity);
                await _context.SaveChangesAsync();



                TempData["MESSAGE"] = "Ticket Assigned successfully";

                //return RedirectToAction("Resolve", new { id = id });
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ticket  could not be assigned" + ex.Message;

                return RedirectToAction("TicketAssignment", new { id = id });
            }
        }


        //public async Task<IActionResult> AssignedTickets(TicketViewModel vm)
        //{
        //    var assignedStatus = await _context
        //    .SystemCodeDetails
        //    .Include(x => x.SystemCode)
        //    .Where(x => x.SystemCode.Code == "ResolutionStatus" && x.Code == "Assigned")
        //    .FirstOrDefaultAsync();

        //    var alltickets = _context.Tickets
        //        .Include(t => t.CreatedBy)
        //        .Include(t => t.SubCategory)
        //        .Include(t => t.Priority)
        //        .Include(t => t.Status)
        //        .Include(t => t.TicketComments)
        //        .Where(x => x.StatusId == assignedStatus.Id)
        //        .OrderBy(x => x.CreatedOn)
        //        .AsQueryable();

        //    if (vm != null)
        //    {
        //        if (!string.IsNullOrEmpty(vm.Title))
        //        {
        //            alltickets = alltickets.Where(x => x.Title == vm.Title);
        //        }

        //        if (!string.IsNullOrEmpty(vm.CreatedById))
        //        {
        //            alltickets = alltickets.Where(x => x.CreatedById == vm.CreatedById);
        //        }

        //        if (vm.StatusId > 0)
        //        {
        //            alltickets = alltickets.Where(x => x.StatusId == vm.StatusId);
        //        }

        //        if (vm.PriorityId > 0)
        //        {
        //            alltickets = alltickets.Where(x => x.PriorityId == vm.PriorityId);
        //        }

        //        if (vm.CategoryId > 0)
        //        {
        //            alltickets = alltickets.Where(x => x.SubCategory.CategoryId == vm.CategoryId);
        //        }
        //    }

        //    vm.Tickets = await alltickets.ToListAsync();


        //    ViewData["PriorityId"] = new SelectList(_context.SystemCodeDetails
        //        .Include(x => x.SystemCode)
        //        .Where(x => x.SystemCode.Code == "Priority"), "Id", "Description");
        //    ViewData["CategoryId"] = new SelectList(_context.TicketCategories, "Id", "Name");
        //    ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "FullName");
        //    ViewData["StatusId"] = new SelectList(_context.SystemCodeDetails
        //        .Include(x => x.SystemCode)
        //        .Where(x => x.SystemCode.Code == "ResolutionStatus"), "Id", "Description");
        //    return View(vm);
        //}

        public async Task<IActionResult> AssignedTickets(TicketViewModel vm)
        {
            var currentUserId = _userManager.GetUserId(User);
            var currentUser = await _userManager.GetUserAsync(User);
            var userRoles = await _userManager.GetRolesAsync(currentUser);

            var assignedStatus = await _context
                .SystemCodeDetails
                .Include(x => x.SystemCode)
                .Where(x => x.SystemCode.Code == "ResolutionStatus" && x.Code == "Assigned")
                .FirstOrDefaultAsync();

            var alltickets = _context.Tickets
                .Include(t => t.CreatedBy)
                .Include(t => t.SubCategory)
                .Include(t => t.Priority)
                .Include(t => t.Status)
                .Include(t => t.TicketComments)
                .Where(x => x.StatusId == assignedStatus.Id)
                .AsQueryable();

            // 🔐 Apply role-based filtering
            if (userRoles.Contains("ADMIN"))
            {
                // No filter - show all assigned tickets
            }
            else if (userRoles.Contains("SUPPORT"))
            {
                alltickets = alltickets.Where(t => t.AssignedToId == currentUserId);
            }
            else if (userRoles.Contains("USER"))
            {
                alltickets = alltickets.Where(t => t.CreatedById == currentUserId);
            }

            // 🔍 Filtering based on form input
            if (vm != null)
            {
                if (!string.IsNullOrEmpty(vm.Title))
                    alltickets = alltickets.Where(x => x.Title == vm.Title);

                if (!string.IsNullOrEmpty(vm.CreatedById))
                    alltickets = alltickets.Where(x => x.CreatedById == vm.CreatedById);

                if (vm.StatusId > 0)
                    alltickets = alltickets.Where(x => x.StatusId == vm.StatusId);

                if (vm.PriorityId > 0)
                    alltickets = alltickets.Where(x => x.PriorityId == vm.PriorityId);

                if (vm.CategoryId > 0)
                    alltickets = alltickets.Where(x => x.SubCategory.CategoryId == vm.CategoryId);
            }

            vm.Tickets = await alltickets.OrderBy(x => x.CreatedOn).ToListAsync();

            ViewData["PriorityId"] = new SelectList(_context.SystemCodeDetails
                .Include(x => x.SystemCode)
                .Where(x => x.SystemCode.Code == "Priority"), "Id", "Description");

            ViewData["CategoryId"] = new SelectList(_context.TicketCategories, "Id", "Name");

            ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "FullName");

            ViewData["StatusId"] = new SelectList(_context.SystemCodeDetails
                .Include(x => x.SystemCode)
                .Where(x => x.SystemCode.Code == "ResolutionStatus"), "Id", "Description");

            return View(vm);
        }


        //public async Task<IActionResult> ClosedTickets(TicketViewModel vm)
        //{
        //    var closedStatus = await _context
        //    .SystemCodeDetails
        //    .Include(x => x.SystemCode)
        //    .Where(x => x.SystemCode.Code == "ResolutionStatus" && x.Code == "Closed")
        //    .FirstOrDefaultAsync();

        //    var alltickets = _context.Tickets
        //       .Include(t => t.CreatedBy)
        //       .Include(t => t.SubCategory)
        //       .Include(t => t.Priority)
        //       .Include(t => t.Status)
        //       .Include(t => t.TicketComments)
        //       .Where(x => x.StatusId == closedStatus.Id)
        //       .OrderBy(x => x.CreatedOn)
        //       .AsQueryable();

        //    if (vm != null)
        //    {
        //        if (!string.IsNullOrEmpty(vm.Title))
        //        {
        //            alltickets = alltickets.Where(x => x.Title == vm.Title);
        //        }

        //        if (!string.IsNullOrEmpty(vm.CreatedById))
        //        {
        //            alltickets = alltickets.Where(x => x.CreatedById == vm.CreatedById);
        //        }

        //        if (vm.StatusId > 0)
        //        {
        //            alltickets = alltickets.Where(x => x.StatusId == vm.StatusId);
        //        }

        //        if (vm.PriorityId > 0)
        //        {
        //            alltickets = alltickets.Where(x => x.PriorityId == vm.PriorityId);
        //        }

        //        if (vm.CategoryId > 0)
        //        {
        //            alltickets = alltickets.Where(x => x.SubCategory.CategoryId == vm.CategoryId);
        //        }
        //    }

        //    vm.Tickets = await alltickets.ToListAsync();


        //    ViewData["PriorityId"] = new SelectList(_context.SystemCodeDetails
        //        .Include(x => x.SystemCode)
        //        .Where(x => x.SystemCode.Code == "Priority"), "Id", "Description");
        //    ViewData["CategoryId"] = new SelectList(_context.TicketCategories, "Id", "Name");
        //    ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "FullName");
        //    ViewData["StatusId"] = new SelectList(_context.SystemCodeDetails
        //        .Include(x => x.SystemCode)
        //        .Where(x => x.SystemCode.Code == "ResolutionStatus"), "Id", "Description");
        //    return View(vm);
        //}


        public async Task<IActionResult> ClosedTickets(TicketViewModel vm)
        {
            var currentUserId = _userManager.GetUserId(User);
            var currentUser = await _userManager.GetUserAsync(User);
            var userRoles = await _userManager.GetRolesAsync(currentUser);

            var closedStatus = await _context
                .SystemCodeDetails
                .Include(x => x.SystemCode)
                .Where(x => x.SystemCode.Code == "ResolutionStatus" && x.Code == "Closed")
                .FirstOrDefaultAsync();

            var alltickets = _context.Tickets
                .Include(t => t.CreatedBy)
                .Include(t => t.SubCategory)
                .Include(t => t.Priority)
                .Include(t => t.Status)
                .Include(t => t.TicketComments)
                .Where(x => x.StatusId == closedStatus.Id)
                .AsQueryable();

            // 🔐 Apply role-based access
            if (userRoles.Contains("ADMIN"))
            {
                // Show all
            }
            else if (userRoles.Contains("SUPPORT"))
            {
                alltickets = alltickets.Where(t => t.AssignedToId == currentUserId);
            }
            else if (userRoles.Contains("USER"))
            {
                alltickets = alltickets.Where(t => t.CreatedById == currentUserId);
            }

            // 🔍 Apply filters if provided
            if (vm != null)
            {
                if (!string.IsNullOrEmpty(vm.Title))
                {
                    alltickets = alltickets.Where(x => x.Title == vm.Title);
                }

                if (!string.IsNullOrEmpty(vm.CreatedById))
                {
                    alltickets = alltickets.Where(x => x.CreatedById == vm.CreatedById);
                }

                if (vm.StatusId > 0)
                {
                    alltickets = alltickets.Where(x => x.StatusId == vm.StatusId);
                }

                if (vm.PriorityId > 0)
                {
                    alltickets = alltickets.Where(x => x.PriorityId == vm.PriorityId);
                }

                if (vm.CategoryId > 0)
                {
                    alltickets = alltickets.Where(x => x.SubCategory.CategoryId == vm.CategoryId);
                }
            }

            vm.Tickets = await alltickets.OrderBy(x => x.CreatedOn).ToListAsync();

            ViewData["PriorityId"] = new SelectList(_context.SystemCodeDetails
                .Include(x => x.SystemCode)
                .Where(x => x.SystemCode.Code == "Priority"), "Id", "Description");

            ViewData["CategoryId"] = new SelectList(_context.TicketCategories, "Id", "Name");

            ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "FullName");

            ViewData["StatusId"] = new SelectList(_context.SystemCodeDetails
                .Include(x => x.SystemCode)
                .Where(x => x.SystemCode.Code == "ResolutionStatus"), "Id", "Description");

            return View(vm);
        }


        //public async Task<IActionResult> ResolvedTickets(TicketViewModel vm)
        //{
        //    var resolvedStatus = await _context
        //    .SystemCodeDetails
        //    .Include(x => x.SystemCode)
        //    .Where(x => x.SystemCode.Code == "ResolutionStatus" && x.Code == "Resolved")
        //    .FirstOrDefaultAsync();

        //    var alltickets = _context.Tickets
        //        .Include(t => t.CreatedBy)
        //        .Include(t => t.SubCategory)
        //        .Include(t => t.Priority)
        //        .Include(t => t.Status)
        //        .Include(t => t.TicketComments)
        //        .Where(x => x.StatusId == resolvedStatus.Id)
        //        .OrderBy(x => x.CreatedOn)
        //        .AsQueryable();

        //    if (vm != null)
        //    {
        //        if (!string.IsNullOrEmpty(vm.Title))
        //        {
        //            alltickets = alltickets.Where(x => x.Title == vm.Title);
        //        }

        //        if (!string.IsNullOrEmpty(vm.CreatedById))
        //        {
        //            alltickets = alltickets.Where(x => x.CreatedById == vm.CreatedById);
        //        }

        //        if (vm.StatusId > 0)
        //        {
        //            alltickets = alltickets.Where(x => x.StatusId == vm.StatusId);
        //        }

        //        if (vm.PriorityId > 0)
        //        {
        //            alltickets = alltickets.Where(x => x.PriorityId == vm.PriorityId);
        //        }

        //        if (vm.CategoryId > 0)
        //        {
        //            alltickets = alltickets.Where(x => x.SubCategory.CategoryId == vm.CategoryId);
        //        }
        //    }

        //    vm.Tickets = await alltickets.ToListAsync();


        //    ViewData["PriorityId"] = new SelectList(_context.SystemCodeDetails
        //        .Include(x => x.SystemCode)
        //        .Where(x => x.SystemCode.Code == "Priority"), "Id", "Description");
        //    ViewData["CategoryId"] = new SelectList(_context.TicketCategories, "Id", "Name");
        //    ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "FullName");
        //    ViewData["StatusId"] = new SelectList(_context.SystemCodeDetails
        //        .Include(x => x.SystemCode)
        //        .Where(x => x.SystemCode.Code == "ResolutionStatus"), "Id", "Description");
        //    return View(vm);
        //}

        public async Task<IActionResult> ResolvedTickets(TicketViewModel vm)
        {
            var currentUserId = _userManager.GetUserId(User);
            var currentUser = await _userManager.GetUserAsync(User);
            var userRoles = await _userManager.GetRolesAsync(currentUser);

            var resolvedStatus = await _context
                .SystemCodeDetails
                .Include(x => x.SystemCode)
                .Where(x => x.SystemCode.Code == "ResolutionStatus" && x.Code == "Resolved")
                .FirstOrDefaultAsync();

            var alltickets = _context.Tickets
                .Include(t => t.CreatedBy)
                .Include(t => t.SubCategory)
                .Include(t => t.Priority)
                .Include(t => t.Status)
                .Include(t => t.TicketComments)
                .Where(x => x.StatusId == resolvedStatus.Id)
                .AsQueryable();

            // 🔐 Role-based ticket filtering
            if (userRoles.Contains("ADMIN"))
            {
                // Show all
            }
            else if (userRoles.Contains("SUPPORT"))
            {
                alltickets = alltickets.Where(t => t.AssignedToId == currentUserId);
            }
            else if (userRoles.Contains("USER"))
            {
                alltickets = alltickets.Where(t => t.CreatedById == currentUserId);
            }

            // 🔍 Filtering based on form input
            if (vm != null)
            {
                if (!string.IsNullOrEmpty(vm.Title))
                {
                    alltickets = alltickets.Where(x => x.Title == vm.Title);
                }

                if (!string.IsNullOrEmpty(vm.CreatedById))
                {
                    alltickets = alltickets.Where(x => x.CreatedById == vm.CreatedById);
                }

                if (vm.StatusId > 0)
                {
                    alltickets = alltickets.Where(x => x.StatusId == vm.StatusId);
                }

                if (vm.PriorityId > 0)
                {
                    alltickets = alltickets.Where(x => x.PriorityId == vm.PriorityId);
                }

                if (vm.CategoryId > 0)
                {
                    alltickets = alltickets.Where(x => x.SubCategory.CategoryId == vm.CategoryId);
                }
            }

            vm.Tickets = await alltickets.OrderBy(x => x.CreatedOn).ToListAsync();

            ViewData["PriorityId"] = new SelectList(_context.SystemCodeDetails
                .Include(x => x.SystemCode)
                .Where(x => x.SystemCode.Code == "Priority"), "Id", "Description");

            ViewData["CategoryId"] = new SelectList(_context.TicketCategories, "Id", "Name");

            ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "FullName");

            ViewData["StatusId"] = new SelectList(_context.SystemCodeDetails
                .Include(x => x.SystemCode)
                .Where(x => x.SystemCode.Code == "ResolutionStatus"), "Id", "Description");

            return View(vm);
        }


        private bool TicketExists(int id)
        {
            return _context.Tickets.Any(e => e.Id == id);
        }
    }
}
