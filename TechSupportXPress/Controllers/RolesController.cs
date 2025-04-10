﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using TechSupportXPress.Data;
using TechSupportXPress.Models;
using TechSupportXPress.ViewModels;

namespace TechSupportXPress.Controllers
{
    public class RolesController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _rolemanager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public RolesController(ApplicationDbContext context,
                               UserManager<ApplicationUser> userManager,
                               RoleManager<IdentityRole> rolemanager,
                               SignInManager<ApplicationUser> signInManager)
        {
            _rolemanager = rolemanager;
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";
            base.OnActionExecuting(context);
        }

        public async Task<ActionResult> Index()
        {
            var roles = await _context.Roles.OrderBy(a => a.Name).ToListAsync();
            return View(roles);
        }

        [HttpGet]
        public async Task<ActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(RolesViewModel vm)
        {
            IdentityRole role = new();
            role.Name = vm.RoleName;

            var result = await _rolemanager.CreateAsync(role);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View(vm);
            }
        }

    }
}
