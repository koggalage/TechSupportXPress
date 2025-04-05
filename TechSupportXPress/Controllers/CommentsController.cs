using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TechSupportXPress.Data;
using TechSupportXPress.Models;
using TechSupportXPress.Repositories.Interfaces;
using TechSupportXPress.Services.Interfaces;

namespace TechSupportXPress.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ICommentService _commentService;
        private readonly IUserRepository _userRepo;
        private readonly ITicketRepository _ticketRepo;
        private readonly IAuditTrailService _auditService;

        public CommentsController(
            ICommentService commentService,
            IUserRepository userRepo,
            ITicketRepository ticketRepo,
            IAuditTrailService auditService)
        {
            _commentService = commentService;
            _userRepo = userRepo;
            _ticketRepo = ticketRepo;
            _auditService = auditService;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";
            base.OnActionExecuting(context);
        }

        // GET: Comments
        public async Task<IActionResult> Index()
        {
            var comments = await _commentService.GetAllAsync();
            return View(comments);
        }

        public async Task<IActionResult> TicketsComments(int id)
        {
            var comments = await _commentService.GetByTicketIdAsync(id);
            return View(comments);
        }


        // GET: Comments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var comment = await _commentService.GetByIdAsync(id.Value);

            if (comment == null)
                return NotFound();

            return View(comment);
        }


        // GET: Comments/Create
        public async Task<IActionResult> Create()
        {
            var users = await _userRepo.GetAllUsersAsync();
            var tickets = await _ticketRepo.GetAllAsync();

            ViewData["CreatedById"] = new SelectList(users, "Id", "FullName");
            ViewData["TicketId"] = new SelectList(tickets, "Id", "Title");

            return View();
        }


        // POST: Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Comment comment)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            var newValues = JsonConvert.SerializeObject(comment, Formatting.Indented);

            comment.CreatedById = userId;

            await _commentService.CreateAsync(comment);

            await _auditService.LogAsync(
                action: "Create",
                userId: userId,
                module: "Comments",
                table: "Comments",
                ipAddress: ipAddress,
                oldValues: null,
                newValues: newValues
    );

            TempData["MESSAGE"] = "Comment successfully Added";
            return RedirectToAction(nameof(Index));
        }


        // GET: Comments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var comment = await _commentService.GetByIdAsync(id.Value);
            if (comment == null)
                return NotFound();

            var users = await _userRepo.GetAllUsersAsync();
            var tickets = await _ticketRepo.GetAllAsync();

            ViewData["CreatedById"] = new SelectList(users, "Id", "FullName", comment.CreatedById);
            ViewData["TicketId"] = new SelectList(tickets, "Id", "Title", comment.TicketId);

            return View(comment);
        }


        // POST: Comments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Comment comment)
        {
            if (id != comment.Id)
                return NotFound();


            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

                //var oldComment = await _commentService.GetByIdAsync(id);
                //var oldValues = JsonConvert.SerializeObject(oldComment, Formatting.Indented);

                comment.ModifiedOn = DateTime.Now;
                comment.ModifiedById = userId;

                await _commentService.UpdateAsync(comment);

               // var newComment = await _commentService.GetByIdAsync(id);
                //var newValues = JsonConvert.SerializeObject(newComment, Formatting.Indented);

                // Log audit
                //await _auditService.LogAsync(
                //    action: "Update",
                //    userId: userId,
                //    module: "Comments",
                //    table: "Comments",
                //    ipAddress: ipAddress,
                //    oldValues: oldValues,
                //    newValues: newValues
                //);

                TempData["MESSAGE"] = "Comment successfully updated.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _commentService.ExistsAsync(comment.Id))
                    return NotFound();

                throw;
            }
        }

        // GET: Comments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var comment = await _commentService.GetByIdAsync(id.Value);
            if (comment == null)
                return NotFound();

            return View(comment);
        }


        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            //var oldComment = await _commentService.GetByIdAsync(id);
            //var oldValues = JsonConvert.SerializeObject(oldComment, Formatting.Indented);

            var result = await _commentService.DeleteAsync(id);

            if (result)
            {
                //await _auditService.LogAsync(
                //    action: "Delete",
                //    userId: userId,
                //    module: "Comments",
                //    table: "Comments",
                //    ipAddress: ipAddress,
                //    oldValues: oldValues,
                //    newValues: null
                //);

                TempData["MESSAGE"] = "Comment deleted successfully.";
            }
            else
                TempData["Error"] = "Failed to delete comment.";

            return RedirectToAction(nameof(Index));
        }


        //private bool CommentExists(int id)
        //{
        //    return _context.Comments.Any(e => e.Id == id);
        //}
    }
}
