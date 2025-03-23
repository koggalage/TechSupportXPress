using Microsoft.AspNetCore.Mvc;

namespace TechSupportXPress.Controllers
{
    using ClosedXML.Excel;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using TechSupportXPress.Data;

    public class ExcelExportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExcelExportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> ExportTicketsList()
        {
            var tickets = await _context.Tickets
                .Include(t => t.Priority)
                .Include(t => t.Status)
                .Include(t => t.SubCategory)
                .Include(t => t.CreatedBy)
                .OrderByDescending(t => t.CreatedOn)
                .ToListAsync();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Tickets");

            // Header
            worksheet.Cell(1, 1).Value = "Title";
            worksheet.Cell(1, 2).Value = "Description";
            worksheet.Cell(1, 3).Value = "Priority";
            worksheet.Cell(1, 4).Value = "Status";
            worksheet.Cell(1, 5).Value = "Sub Category";
            worksheet.Cell(1, 6).Value = "Created By";
            worksheet.Cell(1, 7).Value = "Created On";

            // Data
            for (int i = 0; i < tickets.Count; i++)
            {
                var row = i + 2;
                var ticket = tickets[i];

                worksheet.Cell(row, 1).Value = ticket.Title;
                worksheet.Cell(row, 2).Value = ticket.Description;
                worksheet.Cell(row, 3).Value = ticket.Priority?.Description;
                worksheet.Cell(row, 4).Value = ticket.Status?.Description;
                worksheet.Cell(row, 5).Value = ticket.SubCategory?.Name;
                worksheet.Cell(row, 6).Value = ticket.CreatedBy?.FullName;
                worksheet.Cell(row, 7).Value = ticket.CreatedOn.ToString("yyyy-MM-dd HH:mm");
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Tickets.xlsx");
        }
    }

}
