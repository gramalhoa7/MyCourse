namespace MyCourse.Models;

public class AlternativesRequest
{
    public char Letter { get; set; }
    public string? Content { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsCorrect { get; set; }
}

public class QuestionsRequest
{
    public string? Content { get; set; }
    public string? ImageUrl { get; set; }
    public int TopicId { get; set; }
    public QuestionsDifficulty Difficulty { get; set; }

    public string? ResolutionText { get; set; }
    public string? ResolutionImageUrl { get; set; }
    public string? ResolutionVideoUrl { get; set; }

    // Vazio = questão discursiva (sem alternativas). Com alternativas: de 2 a 10, e exatamente uma correta.
    public List<AlternativesRequest> Alternatives { get; set; } = new();
}
