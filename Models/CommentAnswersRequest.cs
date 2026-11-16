namespace MyCourse.Models;
using System.ComponentModel.DataAnnotations;

public class CommentAnswersRequests
{
    [Required, StringLength(10000)]
    public string Content { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
}
