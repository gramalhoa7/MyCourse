namespace MyCourse.Models
{
    public class VideosRequest
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Url { get; set; } = string.Empty;
        public Guid TopicId { get; set; }
    }
}