namespace MyCourse.Models
{
    public class Questions
    {
        public void NewQuestion(string content, string imageUrl)
        {
            Content = content;
            ImageUrl = imageUrl;
        }

        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
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