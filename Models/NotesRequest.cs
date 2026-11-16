using System.ComponentModel.DataAnnotations;

namespace MyCourse.Models
{
    public class NotesRequest
    {
        [Required, StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [StringLength(10000)]
        public string? Description { get; set; }

        public ColorNotes Color { get; set; }
    }
}
