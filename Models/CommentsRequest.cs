using System.ComponentModel.DataAnnotations;

namespace MyCourse.Models

{
    public class CommentsRequest
    {
        public Guid Id { get; set; }
        
        [Required, StringLength(10000)]
        public string Content { get; set; } = string.Empty;
        public Guid VideoId { get; set; }
        public Guid UserId { get; set; }
        public string? ImageUrl { get; set; }
    }
}