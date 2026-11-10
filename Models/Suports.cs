namespace MyCourse.Models
{
    public class Suports
    {
        public void NewSuport(string title, string content, string imageUrl, Guid matterId)
        {
            Title = title;
            Content = content;
            ImageUrl = imageUrl;
        }

        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
    }
    public enum SuportTopics
    {
        Cadastro = 1,
        FalhaNoPagamento = 2,
        FalhaNoVídeo = 3,
        F
    }
}