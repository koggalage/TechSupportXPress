using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Permissions;
using TechSupportXPress.Data;
using TechSupportXPress.Models;
using TechSupportXPress.ViewModels;

namespace TechSupportXPress.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;


        private readonly ApplicationDbContext _context;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }


        public async Task<IActionResult> Index(TicketDashboardViewModel vm)
        {
            if (!User.Identity.IsAuthenticated)
            {

                return this.Redirect("~/identity/account/login");
            }
            else
            {

                vm.TicketsSummary = await _context.TicketsSummaryView.FirstOrDefaultAsync();


                vm.Tickets = await _context.Tickets
                .Include(t => t.CreatedBy)
                .Include(t => t.SubCategory)
                .Include(t => t.Priority)
                .Include(t => t.Status)
                .Include(t => t.TicketComments)
                .OrderBy(x => x.CreatedOn)
                .ToListAsync();

                return View(vm);
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
