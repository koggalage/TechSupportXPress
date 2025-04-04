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
    public class SystemCodeDetailsController : Controller
    {
        private readonly ISystemCodeDetailService _systemCodeDetailService;
        private readonly IUserRepository _userRepo;
        private readonly ISystemCodeRepository _systemCodeRepo;

        public SystemCodeDetailsController(
            ISystemCodeDetailService systemCodeDetailService,
            IUserRepository userRepo,
            ISystemCodeRepository systemCodeRepo)
        {
            _systemCodeDetailService = systemCodeDetailService;
            _userRepo = userRepo;
            _systemCodeRepo = systemCodeRepo;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";
            base.OnActionExecuting(context);
        }

        // GET: SystemCodeDetails
        public async Task<IActionResult> Index(SystemCodeDetailViewModel vm)
        {
            vm = await _systemCodeDetailService.GetFilteredAsync(vm);

            var systemCodes = await _systemCodeRepo.GetAllAsync();
            var users = await _userRepo.GetAllUsersAsync();

            ViewData["SystemCodeId"] = new SelectList(systemCodes, "Id", "Description");
            ViewData["CreatedById"] = new SelectList(users, "Id", "FullName");

            return View(vm);
        }


        // GET: SystemCodeDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var systemCodeDetail = await _systemCodeDetailService.GetByIdAsync(id.Value);
            if (systemCodeDetail == null)
                return NotFound();

            return View(systemCodeDetail);
        }


        //private bool SystemCodeDetailExists(int id)
        //{
        //    return _context.SystemCodeDetails.Any(e => e.Id == id);
        //}
    }
}
