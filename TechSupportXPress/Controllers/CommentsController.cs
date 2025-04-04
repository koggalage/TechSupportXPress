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

        public CommentsController(
            ICommentService commentService,
            IUserRepository userRepo,
            ITicketRepository ticketRepo)
        {
            _commentService = commentService;
            _userRepo = userRepo;
            _ticketRepo = ticketRepo;
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

            await _commentService.CreateAsync(comment, userId, ipAddress);

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

            if (!ModelState.IsValid)
                return View(comment);

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

                await _commentService.UpdateAsync(comment, userId, ipAddress);

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

            var result = await _commentService.DeleteAsync(id, userId, ipAddress);

            if (result)
                TempData["MESSAGE"] = "Comment deleted successfully.";
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
