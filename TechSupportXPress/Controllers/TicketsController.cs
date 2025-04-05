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
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.SignalR;
using TechSupportXPress.Brokers;
using TechSupportXPress.Services.Interfaces;
using TechSupportXPress.Services;
using Newtonsoft.Json;

namespace TechSupportXPress.Controllers
{
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IHubContext<NotificationHub> _hubContext;

        private readonly ITicketService _ticketService;
        private readonly IDropdownService _dropdownService;
        private readonly INotificationService _notificationService;
        private readonly IFileService _fileService;
        private readonly IAuditTrailService _auditService;
        private readonly ICommentService _commentService;

        public TicketsController(
            ApplicationDbContext context,
            IConfiguration configuration,
            UserManager<ApplicationUser> userManager,
            IHubContext<NotificationHub> hubContext,
            ITicketService ticketService,
            IDropdownService dropdownService,
            INotificationService notificationService,
            IFileService fileService,
            IAuditTrailService auditService,
            ICommentService commentService
            )
        {
            _context = context;
            _configuration = configuration;
            _userManager = userManager;
            _hubContext = hubContext;
            _ticketService = ticketService;
            _dropdownService = dropdownService;
            _notificationService = notificationService;
            _fileService = fileService;
            _auditService = auditService;
            _commentService = commentService;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";
            base.OnActionExecuting(context);
        }

        public async Task<IActionResult> Index(TicketViewModel vm)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var userId = _userManager.GetUserId(User);
                var roles = await _userManager.GetRolesAsync(user);

                vm = await _ticketService.GetFilteredTicketsAsync(userId, roles, vm);

                ViewData["PriorityId"] = await _dropdownService.GetPrioritiesAsync();
                ViewData["CategoryId"] = await _dropdownService.GetCategoriesAsync();
                ViewData["CreatedById"] = await _dropdownService.GetUsersAsync();
                ViewData["StatusId"] = await _dropdownService.GetStatusesAsync();

                return View(vm);
            }
            catch (Exception)
            {

                throw;
            }
        }


        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                if (id == null)
                    return NotFound();

                var vm = await _ticketService.GetTicketDetailsViewModelAsync(id.Value);
                if (vm == null)
                    return NotFound();

                return View(vm);
            }
            catch (Exception)
            {

                throw;
            }
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
        public async Task<IActionResult> Create(TicketCreateViewModel ticketvm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewData["PriorityId"] = await _dropdownService.GetPrioritiesAsync();
                    ViewData["CategoryId"] = await _dropdownService.GetCategoriesAsync();
                    return View(ticketvm);
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var attachmentFileName = await _fileService.SaveAttachmentAsync(ticketvm.Attachment);

                var ticketId = await _ticketService.CreateTicketAsync(ticketvm, userId, attachmentFileName);

                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

                var newValues = JsonConvert.SerializeObject(ticketvm, Formatting.Indented);

                await _auditService.LogAsync("Create", userId, "Tickets", "Tickets", ipAddress, null, newValues);
                await _notificationService.NotifyAdminsAsync($"New Ticket #{ticketId} created: {ticketvm.Title}");

                TempData["MESSAGE"] = "Ticket successfully created";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {

                throw;
            }
        }


        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id == null)
                    return NotFound();

                var vm = await _ticketService.GetTicketEditViewModelAsync(id.Value);
                if (vm == null)
                    return NotFound();

                ViewData["PriorityId"] = await _dropdownService.GetPrioritiesAsync();
                ViewData["CategoryId"] = await _dropdownService.GetCategoriesAsync();
                ViewData["SubCategoryId"] = await _dropdownService.GetSubCategoriesByCategoryIdAsync(vm.CategoryId.Value);

                return View(vm);
            }
            catch (Exception)
            {

                throw;
            }
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TicketViewModel model, IFormFile attachment)
        {
            try
            {
                if (id != model.Id)
                    return NotFound();

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

                // Get original ticket before update
                var originalTicket = await _ticketService.GetTicketByIdAsync(id);
                if (originalTicket == null)
                    return NotFound();

                var oldValues = JsonConvert.SerializeObject(originalTicket, Formatting.Indented);

                var updated = await _ticketService.UpdateTicketAsync(id, model, attachment, userId);

                if (!updated)
                    return NotFound();

                var newValues = JsonConvert.SerializeObject(model, Formatting.Indented);

                await _auditService.LogAsync("Update", userId, "Tickets", "Tickets", ipAddress, oldValues, newValues);

                TempData["MESSAGE"] = "Ticket updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {

                throw;
            }
        }

        // GET: Tickets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null)
                    return NotFound();

                var ticket = await _ticketService.GetTicketForDeleteViewAsync(id.Value);

                if (ticket == null)
                    return NotFound();

                return View(ticket);
            }
            catch (Exception)
            {

                throw;
            }
        }


        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

                var result = await _ticketService.DeleteTicketAsync(id);
                if (!result)
                    return NotFound();

                await _auditService.LogAsync("Delete", userId, "Tickets", "Tickets", ipAddress);

                TempData["MESSAGE"] = "Ticket deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(int id, TicketViewModel vm)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

                await _commentService.AddCommentAsync(id, userId, vm.CommentDescription);

                await _auditService.LogAsync("Create", userId, "Comments", "Comments", ipAddress);

                TempData["MESSAGE"] = "Comment added successfully";
                return RedirectToAction("Details", new { id = id });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to add comment: " + ex.Message;
                return View(vm);
            }
        }

        public async Task<IActionResult> Resolve(int? id)
        {
            try
            {
                if (id == null)
                    return NotFound();

                var vm = await _ticketService.GetTicketResolveViewModelAsync(id.Value);
                if (vm?.TicketDetails == null)
                    return NotFound();

                ViewBag.NextStatus = vm.NextStatus;
                ViewBag.NextStatusId = vm.StatusId;

                return View(vm);
            }
            catch (Exception)
            {

                throw;
            }
        }


        [HttpPost]
        public async Task<IActionResult> ResolvedConfirmed(int id, TicketViewModel vm)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

                var result = await _ticketService.ResolveTicketAsync(id, vm.StatusId, vm.CommentDescription, userId);
                if (!result)
                    return NotFound();

                await _auditService.LogAsync("Update", userId, "Ticket Resolution", "TicketResolution", ipAddress);

                TempData["MESSAGE"] = $"Ticket status changed to {vm.NextStatus}";

                var currentUser = await _userManager.FindByIdAsync(userId);
                var roles = await _userManager.GetRolesAsync(currentUser);

                if (roles.Contains("SUPPORT"))
                {
                    await _notificationService.NotifyAdminsAsync($"Support updated Ticket #{id} status to {vm.NextStatus}");
                }

                if (roles.Contains("SUPPORT") || roles.Contains("ADMIN"))
                {
                    var ticket = await _ticketService.GetTicketByIdAsync(id);
                    if (!string.IsNullOrEmpty(ticket?.CreatedById))
                    {
                        await _notificationService.NotifyUserAsync(ticket.CreatedById, $"Your Ticket #{id} has been updated to status: {vm.NextStatus}");
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IActionResult> Close(int? id)
        {
            try
            {
                if (id == null)
                    return NotFound();

                var vm = await _ticketService.GetTicketCloseViewModelAsync(id.Value);
                if (vm?.TicketDetails == null)
                    return NotFound();

                ViewData["StatusId"] = await _dropdownService.GetResolutionStatusesAsync();
                return View(vm);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> ClosedConfirmed(int id, TicketViewModel vm)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

                var success = await _ticketService.CloseTicketAsync(id, userId);
                if (!success)
                    return NotFound();

                await _auditService.LogAsync("Closed", userId, "Ticket Resolution", "TicketResolution", ipAddress);

                TempData["MESSAGE"] = "Ticket Closed successfully";

                var ticket = await _ticketService.GetTicketByIdAsync(id);

                if (!string.IsNullOrEmpty(ticket?.CreatedById))
                {
                    await _notificationService.NotifyUserAsync(ticket.CreatedById,
                        $"Your ticket #{ticket.Id} has been closed.");
                }

                if (!string.IsNullOrEmpty(ticket?.AssignedToId))
                {
                    await _notificationService.NotifyUserAsync(ticket.AssignedToId,
                        $"Ticket #{ticket.Id} assigned to you has been closed.");
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {

                throw;
            }
        }


        public async Task<IActionResult> ReOpen(int? id)
        {
            try
            {
                if (id == null)
                    return NotFound();

                var vm = await _ticketService.GetTicketReOpenViewModelAsync(id.Value);
                if (vm?.TicketDetails == null)
                    return NotFound();

                ViewData["StatusId"] = await _dropdownService.GetResolutionStatusesAsync();
                return View(vm);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> ReOpenConfirmed(int id, TicketViewModel vm)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

                var success = await _ticketService.ReOpenTicketAsync(id, userId);
                if (!success)
                    return NotFound();

                await _auditService.LogAsync("Re-Open", userId, "Ticket Resolution", "TicketResolution", ipAddress);

                TempData["MESSAGE"] = "Ticket Re-Opened successfully";

                var ticket = await _ticketService.GetTicketByIdAsync(id);

                if (!string.IsNullOrEmpty(ticket?.AssignedToId))
                {
                    await _notificationService.NotifyUserAsync(ticket.AssignedToId,
                        $"Ticket #{ticket.Id}: {ticket.Title} has been re-opened.");
                }

                await _notificationService.NotifyAdminsAsync($"Ticket #{ticket.Id} has been re-opened by the user.");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {

                throw;
            }
        }


        public async Task<IActionResult> TicketAssignment(int? id)
        {
            try
            {
                if (id == null)
                    return NotFound();

                var vm = await _ticketService.GetTicketAssignmentViewModelAsync(id.Value);
                if (vm?.TicketDetails == null)
                    return NotFound();

                ViewData["StatusId"] = await _dropdownService.GetResolutionStatusesAsync();
                ViewData["UserId"] = await _dropdownService.GetSupportUsersAsync();

                return View(vm);
            }
            catch (Exception)
            {

                throw;
            }
        }


        [HttpPost]
        public async Task<IActionResult> AssignedConfirmed(int id, TicketViewModel vm)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

                var success = await _ticketService.AssignTicketAsync(id, vm.AssignedToId, userId);
                if (!success)
                    return NotFound();

                await _auditService.LogAsync("Assigned", userId, "Ticket", "Ticket", ipAddress);

                TempData["MESSAGE"] = "Ticket Assigned successfully";

                await _notificationService.NotifyUserAsync(vm.AssignedToId,
                    $"You have been assigned to Ticket #{id}");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ticket could not be assigned: " + ex.Message;
                return RedirectToAction("TicketAssignment", new { id = id });
            }
        }

        public async Task<IActionResult> AssignedTickets(TicketViewModel vm)
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var resultVm = await _ticketService.GetAssignedTicketsAsync(vm, currentUser);

                ViewData["PriorityId"] = await _dropdownService.GetPrioritiesAsync();
                ViewData["CategoryId"] = await _dropdownService.GetCategoriesAsync();
                ViewData["CreatedById"] = await _dropdownService.GetUsersAsync();
                ViewData["StatusId"] = await _dropdownService.GetResolutionStatusesAsync();

                return View(resultVm);
            }
            catch (Exception)
            {

                throw;
            }
        }


        public async Task<IActionResult> ClosedTickets(TicketViewModel vm)
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var resultVm = await _ticketService.GetClosedTicketsAsync(vm, currentUser);

                ViewData["PriorityId"] = await _dropdownService.GetPrioritiesAsync();
                ViewData["CategoryId"] = await _dropdownService.GetCategoriesAsync();
                ViewData["CreatedById"] = await _dropdownService.GetUsersAsync();
                ViewData["StatusId"] = await _dropdownService.GetResolutionStatusesAsync();

                return View(resultVm);
            }
            catch (Exception)
            {

                throw;
            }
        }


        public async Task<IActionResult> ResolvedTickets(TicketViewModel vm)
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var resultVm = await _ticketService.GetResolvedTicketsAsync(vm, currentUser);

                ViewData["PriorityId"] = await _dropdownService.GetPrioritiesAsync();
                ViewData["CategoryId"] = await _dropdownService.GetCategoriesAsync();
                ViewData["CreatedById"] = await _dropdownService.GetUsersAsync();
                ViewData["StatusId"] = await _dropdownService.GetResolutionStatusesAsync();

                return View(resultVm);
            }
            catch (Exception)
            {

                throw;
            }
        }


        private bool TicketExists(int id)
        {
            return _context.Tickets.Any(e => e.Id == id);
        }
    }
}
