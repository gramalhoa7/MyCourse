using System.ComponentModel.DataAnnotations;

namespace MyCourse.Models
{
    public class Comments
    {
        public void NewComment(Guid videoId, Guid userId, string content, string? imageUrl)
        {
            Content = content;
            VideoId = videoId;
            UserId = userId;
            ImageUrl = imageUrl;

            Id = Guid.NewGuid();
        }
        public void UpdateComment(string content, string? imageUrl)
        {
            Content = content;
            ImageUrl = imageUrl;
        }
        public Guid Id { get; set; }
        public Guid VideoId { get; set; }
        public Videos Video { get; set; } = null!;
        public Guid UserId { get; set; }
        public Users User { get; set; } = null!;
        
        [Required, StringLength(10000)]
        public string Content { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}