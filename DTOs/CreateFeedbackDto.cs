namespace SmartFeedbackPortal.API.DTOs
{
    public class CreateFeedbackDto
    {
        public string Category { get; set; }
        public string Content { get; set; }
        public string ImageUrl { get; set; }  // Optional
    }
}
