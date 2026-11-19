using System.ComponentModel.DataAnnotations;

namespace MyCourse.Models;

public class SupportsRequest
{
    [Required]
    public int SubjectId { get; set; }

    [Required, StringLength(10000, MinimumLength = 1)]
    public string Text { get; set; } = string.Empty;
}