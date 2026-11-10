namespace MyCourse.Models
{
    public class Videos
    {
        public Videos(string title, string description, string url, Guid topicId)
        {
            Title = title;
            Description = description;
            Url = url;
            TopicId = topicId;
            
            Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Url { get; set; } = string.Empty;
        public Guid TopicId { get; set; }
        public Topics Topic { get; set; } = null!;
    }
}