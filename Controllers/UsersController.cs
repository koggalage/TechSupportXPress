using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TechSupportXPress.Data;
using TechSupportXPress.Models;
using TechSupportXPress.ViewModels;

namespace TechSupportXPress.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _rolemanager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context, 
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

        // GET: UsersController
        public async Task<ActionResult> Index()
        {
            var users = await _userManager.Users
                .Where(u => !u.IsDeleted)
                .ToListAsync();

            var userRoles = new List<UserWithRoleViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRoles.Add(new UserWithRoleViewModel
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    MiddleName = user.MiddleName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    Email = user.Email,
                    Gender = user.Gender,
                    Role = roles.FirstOrDefault() ?? "N/A"
                });
            }

            return View(userRoles);
        }



        // GET: UsersController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: UsersController/Create
        public async Task<ActionResult> Create()
        {
            var roles = await _rolemanager.Roles
                .Select(r => new SelectListItem { Value = r.Name, Text = r.Name })
                .ToListAsync();

            var viewModel = new UserCreateViewModel
            {
                Roles = roles
            };

            return View(viewModel);
        }

        // POST: UsersController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(UserCreateViewModel model)
        {
            //if (!ModelState.IsValid)
            //{
            //    model.Roles = await _rolemanager.Roles
            //        .Select(r => new SelectListItem { Value = r.Name, Text = r.Name })
            //        .ToListAsync();
            //    return View(model);
            //}

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                ApplicationUser registereduser = new()
                {
                    FirstName = model.FirstName,
                    UserName = model.UserName,
                    MiddleName = model.MiddleName,
                    LastName = model.LastName,
                    NormalizedUserName = model.UserName.ToUpper(),
                    Email = model.Email,
                    EmailConfirmed = true,
                    Gender = model.Gender,
                    City = model.City,
                    Country = model.Country,
                    PhoneNumber = model.PhoneNumber
                };

                var result = await _userManager.CreateAsync(registereduser, model.Password);

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    user.EmailConfirmed = true;
                    await _userManager.UpdateAsync(user);

                    if (!string.IsNullOrEmpty(model.Role))
                    {
                        await _userManager.AddToRoleAsync(registereduser, model.Role);
                    }

                    var activity = new AuditTrail
                    {
                        Action = "Create",
                        TimeStamp = DateTime.Now,
                        IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                        UserId = userId,
                        Module = "Users",
                        AffectedTable = "Users"
                    };

                    _context.Add(activity);
                    await _context.SaveChangesAsync();

                    TempData["MESSAGE"] = "User is successfully Created";
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }

                    model.Roles = await _rolemanager.Roles
                        .Select(r => new SelectListItem { Value = r.Name, Text = r.Name })
                        .ToListAsync();
                    return View(model);
                }
            }
            catch
            {
                model.Roles = await _rolemanager.Roles
                    .Select(r => new SelectListItem { Value = r.Name, Text = r.Name })
                    .ToListAsync();
                return View(model);
            }
        }

        // GET: UsersController/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var userRoles = await _userManager.GetRolesAsync(user);
            var selectedRole = userRoles.FirstOrDefault();

            var roles = await _rolemanager.Roles
                .Select(r => new SelectListItem
                {
                    Value = r.Name,
                    Text = r.Name,
                    Selected = (r.Name == selectedRole) // <-- Set selected
                })
                .ToListAsync();

            var viewModel = new UserCreateViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,
                Gender = user.Gender,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Country = user.Country,
                City = user.City,
                UserName = user.UserName,
                Role = selectedRole,
                Roles = roles
            };

            return View(viewModel);
        }



        // POST: UsersController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id, UserCreateViewModel model)
        {
            if (id != model.Id)
                return NotFound();

            //if (!ModelState.IsValid)
            //{
            //    model.Roles = await _rolemanager.Roles
            //        .Select(r => new SelectListItem { Value = r.Name, Text = r.Name })
            //        .ToListAsync();
            //    return View(model);
            //}

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            // Update user fields
            user.FirstName = model.FirstName;
            user.MiddleName = model.MiddleName;
            user.LastName = model.LastName;
            user.Gender = model.Gender;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            user.Country = model.Country;
            user.City = model.City;
            user.UserName = model.UserName;
            user.NormalizedUserName = model.UserName.ToUpper();

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Failed to update user.");
                return View(model);
            }

            // Update role
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            if (!string.IsNullOrEmpty(model.Role))
                await _userManager.AddToRoleAsync(user, model.Role);

            TempData["MESSAGE"] = "User updated successfully";
            return RedirectToAction(nameof(Index));
        }


        // GET: UsersController/Delete/5
        // GET: UsersController/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            return View(user); // You can also pass a simplified view model
        }


        // POST: UsersController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound();

            // Soft delete by marking as deleted and prefixing email and username
            user.IsDeleted = true;

            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            user.Email = $"deleted-{timestamp}-{user.Email}";
            user.UserName = $"deleted-{timestamp}-{user.UserName}";
            user.NormalizedEmail = user.Email.ToUpperInvariant();
            user.NormalizedUserName = user.UserName.ToUpperInvariant();

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                TempData["Error"] = "Failed to delete the user.";
                return RedirectToAction(nameof(Index));
            }

            // Optional: log audit
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _context.AuditTrails.Add(new AuditTrail
            {
                Action = "Soft Delete",
                TimeStamp = DateTime.Now,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                UserId = currentUserId,
                Module = "Users",
                AffectedTable = "AspNetUsers"
            });
            await _context.SaveChangesAsync();

            TempData["MESSAGE"] = "User deleted successfully (soft delete).";
            return RedirectToAction(nameof(Index));
        }



        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(user);

            var vm = new UserProfileViewModel
            {
                Id = user.Id,
                FullName = $"{user.FirstName} {user.LastName}",
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Gender = user.Gender,
                Roles = roles.ToList()
            };

            return View(vm);
        }
    }
}
