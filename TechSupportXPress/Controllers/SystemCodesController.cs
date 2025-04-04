using System;
using System.Collections.Generic;
using System.Linq;
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
using TechSupportXPress.ViewModels;

namespace TechSupportXPress.Controllers
{
    public class SystemCodesController : Controller
    {
        private readonly ISystemCodeService _systemCodeService;
        private readonly IUserRepository _userRepo;

        public SystemCodesController(ISystemCodeService systemCodeService, IUserRepository userRepo)
        {
            _systemCodeService = systemCodeService;
            _userRepo = userRepo;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";
            base.OnActionExecuting(context);
        }

        // GET: SystemCodes
        public async Task<IActionResult> Index(SystemCodeViewModel vm)
        {
            vm = await _systemCodeService.GetFilteredAsync(vm);

            var users = await _userRepo.GetAllUsersAsync();
            ViewData["CreatedById"] = new SelectList(users, "Id", "FullName");

            return View(vm);
        }


        // GET: SystemCodes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var systemCode = await _systemCodeService.GetByIdAsync(id.Value);
            if (systemCode == null)
                return NotFound();

            return View(systemCode);
        }


        //private bool SystemCodeExists(int id)
        //{
        //    return _context.SystemCodes.Any(e => e.Id == id);
        //}
    }
}
