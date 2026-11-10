namespace MyCourse.Models
{
    public class Alternatives
    {
        public void NewAlternative(char letter, string content, string imageUrl, bool isCorrect, Guid questionId)
        {
            Letter = letter;
            Content = content;
            ImageUrl = imageUrl;
            IsCorrect = isCorrect;
            QuestionId = questionId;
        }

        public int Id { get; set; }
        public char Letter { get; set; }
        public string Content { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public Guid QuestionId { get; set; }
        public Questions Question { get; set; } = null!;
    }
}