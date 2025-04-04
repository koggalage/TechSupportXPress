using System;
using System.Collections.Generic;
using System.Linq;
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
    public class AuditTrailsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuditTrailService _auditTrailService;
        private readonly IUserRepository _userRepo;

        public AuditTrailsController(
            ApplicationDbContext context,
            IAuditTrailService auditTrailService,
            IUserRepository userRepo
            )
        {
            _context = context;
            _auditTrailService = auditTrailService;
            _userRepo = userRepo;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";
            base.OnActionExecuting(context);
        }

        // GET: AuditTrails
        public async Task<IActionResult> Index()
        {
            var data = await _auditTrailService.GetAllAsync();
            return View(data);
        }


        // GET: AuditTrails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var auditTrail = await _auditTrailService.GetByIdAsync(id.Value);

            if (auditTrail == null)
                return NotFound();

            return View(auditTrail);
        }

    }
}
