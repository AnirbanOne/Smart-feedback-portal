using Microsoft.AspNetCore.Identity;

namespace SmartFeedbackPortal.API.Models
{
    public class User : IdentityUser
    {
        public string FullName { get; set; }
        public ICollection<Feedback> Feedbacks { get; set; }
    }
}
