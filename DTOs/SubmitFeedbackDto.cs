using Microsoft.AspNetCore.Http;

namespace SmartFeedbackPortal.API.DTOs
{
    public class SubmitFeedbackDto
    {
        public string Category { get; set; }
        public string Content { get; set; }
        public IFormFile? Image { get; set; }
    }
}
