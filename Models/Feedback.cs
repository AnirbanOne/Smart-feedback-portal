namespace SmartFeedbackPortal.API.Models
{
    public class Feedback
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        public string Sentiment { get; set; }
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        public string UserId { get; set; }
        public User User { get; set; }
    }
}
