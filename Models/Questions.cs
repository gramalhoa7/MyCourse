namespace MyCourse.Models
{
    public class Questions
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;

        // A matéria não é guardada aqui: vem do tópico (Topic -> Branch -> Matter)
        public int TopicId { get; set; }
        public Topics Topic { get; set; } = null!;

        public QuestionsDifficulty Difficulty { get; set; }

        // Resolução: aparece na área de resolução do site (todos os campos são opcionais)
        public string? ResolutionText { get; set; }
        public string? ResolutionImageUrl { get; set; }
        public string? ResolutionVideoUrl { get; set; }

        public List<Alternatives> Alternatives { get; set; } = new();   // atalho, não cria coluna
    }
    public enum QuestionsDifficulty
    {
        MuitoFácil = 1,
        Fácil = 2,
        Médio = 3,
        Difícil = 4,
        MuitoDifícil = 5
    }
}
