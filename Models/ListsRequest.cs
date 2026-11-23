using System.ComponentModel.DataAnnotations;

namespace MyCourse.Models;

// Lista oficial, criada pela equipe
public class ListsRequest
{
    [Required, StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    public Matters Matter { get; set; }
    public bool PreList { get; set; }

    // Para quais objetivos a lista aparece. Vazio = aparece para todos.
    public List<Objectives> Objectives { get; set; } = new();

    // As questões escolhidas, na ordem em que aparecem na lista
    public List<int> QuestionIds { get; set; } = new();
}

// Lista personalizada, criada pelo aluno
public class CustomListsRequest
{
    [StringLength(150)]
    public string? Title { get; set; }

    public Matters Matter { get; set; }
    public int Count { get; set; }

    // Vazio = todas as dificuldades
    public List<QuestionsDifficulty> Difficulties { get; set; } = new();

    // Vazio = todos os assuntos da matéria
    public List<int> TopicIds { get; set; } = new();
}
