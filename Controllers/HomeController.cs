using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<HomeController> _logger;


        private readonly ApplicationDbContext _context;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }


        //public async Task<IActionResult> Index(TicketDashboardViewModel vm)
        //{
        //    if (!User.Identity.IsAuthenticated)
        //    {

        //        return this.Redirect("~/identity/account/login");
        //    }
        //    else
        //    {

        //        vm.TicketsSummary = await _context.TicketsSummaryView.FirstOrDefaultAsync();


        //        vm.Tickets = await _context.Tickets
        //        .Include(t => t.CreatedBy)
        //        .Include(t => t.SubCategory)
        //        .Include(t => t.Priority)
        //        .Include(t => t.Status)
        //        .Include(t => t.TicketComments)
        //        .OrderBy(x => x.CreatedOn)
        //        .ToListAsync();

        //        return View(vm);
        //    }
        //}

        public async Task<IActionResult> Index(TicketDashboardViewModel vm)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Redirect("~/identity/account/login");
            }

            var currentUserId = _userManager.GetUserId(User);
            var currentUser = await _userManager.GetUserAsync(User);
            var userRoles = await _userManager.GetRolesAsync(currentUser);

            var ticketsQuery = _context.Tickets
                .Include(t => t.Status)
                .Include(t => t.CreatedBy)
                .Include(t => t.SubCategory)
                .Include(t => t.Priority)
                .Include(t => t.TicketComments)
                .AsQueryable();

            if (userRoles.Contains("SUPPORT"))
            {
                ticketsQuery = ticketsQuery.Where(t => t.AssignedToId == currentUserId);
            }
            else if (userRoles.Contains("USER"))
            {
                ticketsQuery = ticketsQuery.Where(t => t.CreatedById == currentUserId);
            }

            var allTickets = await ticketsQuery.ToListAsync();

            vm.Tickets = allTickets.OrderBy(x => x.CreatedOn).ToList();

            vm.TicketsSummary = new TicketsSummaryView
            {
                TotalTickets = allTickets.Count,
                PendingTickets = allTickets.Count(t => t.Status.Code == "Pending"),
                AssignedTickets = allTickets.Count(t => t.Status.Code == "Assigned"),
                ResolvedTickets = allTickets.Count(t => t.Status.Code == "Resolved"),
                ClosedTickets = allTickets.Count(t => t.Status.Code == "Closed"),
                ReOpenedTickets = allTickets.Count(t => t.Status.Code == "ReOpened")
            };

            // Last 7 days ticket count grouped by date
            var last7Days = Enumerable.Range(0, 7)
    .Select(i => DateTime.Today.AddDays(-i))
    .OrderBy(d => d)
    .ToList();

            var priorities = await _context.SystemCodeDetails
                .Where(p => p.SystemCode.Code == "Priority")
                .ToListAsync();

            var chartData = new Dictionary<string, List<int>>();
            foreach (var priority in priorities)
            {
                var dataPoints = last7Days.Select(date =>
                    allTickets.Count(t =>
                        t.PriorityId == priority.Id &&
                        t.CreatedOn.Date == date.Date
                    )
                ).ToList();

                chartData[priority.Description] = dataPoints;
            }

            ViewBag.ChartLabels = string.Join(",", last7Days.Select(d => $"'{d:MMM dd}'"));
            ViewBag.ChartData = chartData;


            var openStatuses = new[] { "Pending", "Assigned", "ReOpened" };
            var closedCount = allTickets.Count(t => t.Status.Code == "Closed");
            var openCount = allTickets.Count(t => openStatuses.Contains(t.Status.Code));

            ViewBag.OpenTicketCount = openCount;
            ViewBag.ClosedTicketCount = closedCount;

            return View(vm);
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
