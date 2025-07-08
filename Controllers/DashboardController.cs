using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartFeedbackPortal.API.Data;

namespace SmartFeedbackPortal.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var feedbacks = await _context.Feedbacks.ToListAsync();

            var byCategory = feedbacks
                .GroupBy(f => f.Category)
                .ToDictionary(g => g.Key, g => g.Count());

            var bySentiment = feedbacks
                .GroupBy(f => f.Sentiment)
                .ToDictionary(g => g.Key, g => g.Count());

            var byDate = feedbacks
                .GroupBy(f => f.SubmittedAt.Date)
                .OrderBy(g => g.Key)
                .ToDictionary(g => g.Key.ToShortDateString(), g => g.Count());

            return Ok(new
            {
                Category = byCategory,
                Sentiment = bySentiment,
                Date = byDate
            });
        }
    }
}
