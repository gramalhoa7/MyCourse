namespace MyCourse.Models;
using System.ComponentModel.DataAnnotations;

public class CommentAnswersRequests
{
    public Guid CommentId { get; set; }
    public Guid UserId { get; set; }
    
    [Required, StringLength(10000)]
    public string Content { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
}