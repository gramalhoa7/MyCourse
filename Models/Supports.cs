namespace MyCourse.Models;

public class Supports
{
    public Supports(int subjectId, string text, Guid userId)
    {
        Id = Guid.NewGuid();
        SubjectId = subjectId;
        Text = text;
        UserId = userId;
    }

    public Guid Id { get; set; }

    public int SubjectId { get; set; }
    public Subjects? Subject { get; set; }

    public Guid UserId { get; set; }
    public Users? User { get; set; }

    public string Text { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<SupportAnswers> Answers { get; set; } = new List<SupportAnswers>();
}
