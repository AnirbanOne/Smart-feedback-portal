using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartFeedbackPortal.API.Data;

namespace SmartFeedbackPortal.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class FeedbackExportController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FeedbackExportController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("excel")]
        public async Task<IActionResult> ExportFeedback()
        {
            var feedbacks = await _context.Feedbacks
                .Include(f => f.User)
                .ToListAsync();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Feedback");

            worksheet.Cell(1, 1).Value = "User";
            worksheet.Cell(1, 2).Value = "Category";
            worksheet.Cell(1, 3).Value = "Content";
            worksheet.Cell(1, 4).Value = "Sentiment";
            worksheet.Cell(1, 5).Value = "Date";

            int row = 2;
            foreach (var fb in feedbacks)
            {
                worksheet.Cell(row, 1).Value = fb.User.FullName;
                worksheet.Cell(row, 2).Value = fb.Category;
                worksheet.Cell(row, 3).Value = fb.Content;
                worksheet.Cell(row, 4).Value = fb.Sentiment;
                worksheet.Cell(row, 5).Value = fb.SubmittedAt.ToString("g");
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);

            return File(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "FeedbackReport.xlsx");
        }
    }
}
