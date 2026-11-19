using System.ComponentModel.DataAnnotations;

namespace MyCourse.Models;

public class SupportAnswers
{
    public SupportAnswers(Guid supportId, Guid userId, string text)
    {
        Id = Guid.NewGuid();
        SupportId = supportId;
        UserId = userId;
        Text = text;
    }

    public Guid Id { get; set; }

    public Guid SupportId { get; set; }
    public Supports Support { get; set; } = null!;

    public Guid UserId { get; set; }
    public Users User { get; set; } = null!;

    [Required, StringLength(10000)]
    public string Text { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}