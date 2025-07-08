namespace SmartFeedbackPortal.API.DTOs
{
    public class FeedbackResponseDto
    {
        public string Category { get; set; }
        public string Content { get; set; }
        public string Sentiment { get; set; }
        public DateTime SubmittedAt { get; set; }

        public UserDto User { get; set; }
    }

    public class UserDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
    }
}
