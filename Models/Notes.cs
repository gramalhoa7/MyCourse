namespace MyCourse.Models
{
    public class Notes
    {
        public void NewNote(string title, string? description, ColorNotes color, Guid videoId, Guid userId)
        {
            Title = title;
            Description = description;
            Color = color;
            VideoId = videoId;
            UserId = userId;

            Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public ColorNotes Color { get; set; } 
        public Guid VideoId { get; set; } //para a pessoa ir direto para o video sem procurar usar url do video
        public Videos Video { get; set; } = null!;
        public Guid UserId { get; set; }
        public Users User { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
    public enum ColorNotes
    {
        Red,
        Orange,
        Yellow,
        Lime,
        Green,
        Cyan,
        LightBlue,
        Blue,
        Purple,
        Pink,
        Brown,
        Gray,
        DarkGray,
        Black,
        White
    }
}