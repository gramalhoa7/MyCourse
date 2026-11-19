namespace MyCourse.Models;

public class Subjects
{
    public Subjects(string subject)
    {
        Subject = subject;
    }
    public int Id { get; set; }
    public string? Subject { get; set; } 
}