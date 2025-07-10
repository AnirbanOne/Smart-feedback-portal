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
        private readonly AzureBlobStorageService _blobService;

        public FeedbackController(
            ApplicationDbContext context,
            AzureAISentimentService sentimentService,
            AzureBlobStorageService blobService)
        {
            _context = context;
            _sentimentService = sentimentService;
            _blobService = blobService;
        }

        // ✅ Submit Feedback
        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> SubmitFeedback([FromForm] SubmitFeedbackDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string? imageUrl = null;

            if (dto.Image != null)
            {
                imageUrl = await _blobService.UploadFileAsync(dto.Image);
            }

            var sentiment = await _sentimentService.AnalyzeSentimentAsync(dto.Content);

            var feedback = new Feedback
            {
                Category = dto.Category,
                Content = dto.Content,
                ImageUrl = imageUrl,
                Sentiment = sentiment,
                UserId = userId
            };

            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Feedback submitted successfully", sentiment });
        }

        // ✅ Get My Feedbacks
        [HttpGet("my")]
        [Authorize(Roles = "User,Admin")]
        public async Task<ActionResult<IEnumerable<FeedbackResponseDto>>> GetMyFeedback()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var feedbacks = await _context.Feedbacks
                .Where(f => f.UserId == userId)
                .Include(f => f.User)
                .OrderByDescending(f => f.SubmittedAt)
                .Select(f => new FeedbackResponseDto
                {
                    Category = f.Category,
                    Content = f.Content,
                    Sentiment = f.Sentiment,
                    SubmittedAt = f.SubmittedAt,
                    User = new UserDto
                    {
                        FullName = f.User.FullName,
                        Email = f.User.Email
                    }
                })
                .ToListAsync();

            return Ok(feedbacks);
        }

        // ✅ Admin: View All Feedbacks
        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<FeedbackResponseDto>>> GetAllFeedback()
        {
            var feedbacks = await _context.Feedbacks
                .Include(f => f.User)
                .OrderByDescending(f => f.SubmittedAt)
                .Select(f => new FeedbackResponseDto
                {
                    Category = f.Category,
                    Content = f.Content,
                    Sentiment = f.Sentiment,
                    SubmittedAt = f.SubmittedAt,
                    User = new UserDto
                    {
                        FullName = f.User.FullName,
                        Email = f.User.Email
                    }
                })
                .ToListAsync();

            return Ok(feedbacks);
        }
    }
}
