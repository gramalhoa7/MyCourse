namespace MyCourse.Models;

public class Lists
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }

    // Uma lista nunca mistura matérias
    public Matters Matter { get; set; }
    public bool PreList { get; set; }

    // null = lista oficial (criada pela equipe). Preenchido = lista personalizada deste aluno.
    public Guid? OwnerId { get; set; }
    public Users? Owner { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<ListsQuestions> Questions { get; set; } = new();
    public List<ListsObjectives> Objectives { get; set; } = new();
}
public class ListsQuestions
{
    public int Id { get; set; }
    public int ListId { get; set; }
    public Lists List { get; set; } = null!;
    public int QuestionId { get; set; }
    public Questions Question { get; set; } = null!;
    public int Order { get; set; }   // posição da questão na lista
}
public class ListsObjectives
{
    public int Id { get; set; }
    public int ListId { get; set; }
    public Lists List { get; set; } = null!;
    public Objectives Objective { get; set; }
}
