using System.ComponentModel.DataAnnotations;

namespace MyCourse.Models;

public class SupportAnswersRequest
{
    [Required, StringLength(10000, MinimumLength = 1)]
    public string Text { get; set; } = string.Empty;
}