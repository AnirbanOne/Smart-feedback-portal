using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartFeedbackPortal.API.Data;
using SmartFeedbackPortal.API.DTOs;
using SmartFeedbackPortal.API.Models;
using SmartFeedbackPortal.API.Services;
using System.Security.Claims;

namespace SmartFeedbackPortal.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedbackController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly AzureAISentimentService _sentimentService;

        public FeedbackController(ApplicationDbContext context, AzureAISentimentService sentimentService)
        {
            _context = context;
            _sentimentService = sentimentService;
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> SubmitFeedback(CreateFeedbackDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var sentiment = await _sentimentService.AnalyzeSentimentAsync(dto.Content);

            var feedback = new Feedback
            {
                Category = dto.Category,
                Content = dto.Content,
                ImageUrl = dto.ImageUrl,
                Sentiment = sentiment,
                UserId = userId
            };

            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Feedback submitted successfully", sentiment });
        }

        [HttpGet("my")]
        [Authorize(Roles = "User,Admin")]
        public async Task<ActionResult<IEnumerable<FeedbackResponseDto>>> GetMyFeedback()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var feedbacks = await _context.Feedbacks
                .Where(f => f.UserId == userId)
                .Select(f => new FeedbackResponseDto
                {
                    Category = f.Category,
                    Content = f.Content,
                    Sentiment = f.Sentiment,
                    SubmittedAt = f.SubmittedAt
                })
                .ToListAsync();

            return Ok(feedbacks);
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<FeedbackResponseDto>>> GetAllFeedback()
        {
            var feedbacks = await _context.Feedbacks
                .Include(f => f.User)
                .Select(f => new FeedbackResponseDto
                {
                    Category = f.Category,
                    Content = f.Content,
                    Sentiment = f.Sentiment,
                    SubmittedAt = f.SubmittedAt,
                    User = new UserDto
                    {
                        FullName = f.User.FullName,
                    }
                })
                .ToListAsync();

            return Ok(feedbacks);
        }
    }
}
