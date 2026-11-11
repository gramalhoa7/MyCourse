using System.ComponentModel.DataAnnotations;

namespace MyCourse.Models
{
    public class CommentAnswers
    {
        public CommentAnswers(Guid commentId, Guid userId, string content, string? imageUrl)
        {
            Content = content;
            CommentId = commentId;
            UserId = userId;
            ImageUrl = imageUrl;

            Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }
        public Guid CommentId { get; set; }
        public Comments Comment { get; set; } = null!;
        public Guid UserId { get; set; }
        public Users User { get; set; } = null!;
        
        [Required, StringLength(10000)]
        public string Content { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}