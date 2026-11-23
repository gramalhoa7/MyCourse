using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using MyCourse.Data;
using MyCourse.Models;
using MyCourse.Services;

namespace MyCourse.Routes;

public static class ListsRoute
{
    private const int MaxQuestionsPerList = 100;
    private const int MaxCustomQuestions = 50;
    private const int MaxCustomListsPerUser = 50;

    // ---------- Funções auxiliares ----------

    // Devolve a mensagem de erro, ou null se estiver tudo certo
    private static async Task<string?> ValidateAsync(DBContext context, ListsRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Title))
            return "Title cannot be empty.";
        if (req.Title.Length > 150)
            return "Title exceeds the maximum length of 150 characters.";
        if (req.Description is not null && req.Description.Length > 2000)
            return "Description exceeds the maximum length of 2,000 characters.";
        if (!Enum.IsDefined(req.Matter))
            return "Invalid matter.";
        if (req.Objectives.Any(o => !Enum.IsDefined(o)))
            return "Invalid objective.";
        if (req.QuestionIds.Count == 0)
            return "The list needs at least one question.";
        if (req.QuestionIds.Count > MaxQuestionsPerList)
            return $"A list can have at most {MaxQuestionsPerList} questions.";
        if (req.QuestionIds.Distinct().Count() != req.QuestionIds.Count)
            return "The same question cannot appear twice in a list.";

        var found = await context.Questions
            .Where(q => req.QuestionIds.Contains(q.Id))
            .Select(q => new { q.Id, Matter = q.Topic.Branch.Matter })
            .ToListAsync();

        if (found.Count != req.QuestionIds.Count)
            return "One or more questions do not exist.";
        if (found.Any(f => f.Matter != req.Matter))
            return "All questions must belong to the matter chosen for the list.";

        return null;
    }

    private static List<ListsQuestions> MakeQuestions(ListsRequest req) =>
        req.QuestionIds
            .Select((questionId, index) => new ListsQuestions { QuestionId = questionId, Order = index + 1 })
            .ToList();

    private static List<ListsObjectives> MakeObjectives(ListsRequest req) =>
        req.Objectives
            .Distinct()
            .Select(objective => new ListsObjectives { Objective = objective })
            .ToList();

    // Quem pode ver a lista:
    //  - lista personalizada: só o dono
    //  - lista oficial: equipe vê todas; aluno vê as sem objetivo marcado ou as do seu objetivo
    private static bool CanView(Lists list, Guid userId, bool isStaff, Objectives? objective)
    {
        if (list.OwnerId != null) return list.OwnerId == userId;
        if (isStaff) return true;
        if (list.Objectives.Count == 0) return true;
        return objective != null && list.Objectives.Any(o => o.Objective == objective);
    }

    private static Task<List<ListsQuestions>> LoadItemsAsync(DBContext context, int listId) =>
        context.ListsQuestions
            .Where(lq => lq.ListId == listId)
            .OrderBy(lq => lq.Order)
            .Include(lq => lq.Question).ThenInclude(q => q.Topic)
            .Include(lq => lq.Question).ThenInclude(q => q.Alternatives)
            .AsSplitQuery()
            .ToListAsync();

    // O gabarito (IsCorrect) só vai para a equipe. O aluno nunca recebe.
    private static object ToDetail(Lists list, List<ListsQuestions> items, bool isStaff) => new
    {
        list.Id,
        list.Title,
        list.Description,
        list.Matter,
        list.PreList,
        list.CreatedAt,
        IsOfficial = list.OwnerId == null,
        Questions = items.Select(lq => new
        {
            lq.Order,
            lq.Question.Id,
            lq.Question.Content,
            lq.Question.ImageUrl,
            lq.Question.TopicId,
            Topic = lq.Question.Topic.Name,
            lq.Question.Difficulty,
            Alternatives = lq.Question.Alternatives
                .OrderBy(a => a.Letter)
                .Select(a => isStaff
                    ? (object)new { a.Id, a.Letter, a.Content, a.ImageUrl, a.IsCorrect }
                    : new { a.Id, a.Letter, a.Content, a.ImageUrl })
        })
    };

    // ---------- Rotas ----------

    public static void ListsRoutes(this WebApplication app)
    {
        // Listas oficiais que o aluno pode ver (filtros: ?matter=Matemática&preList=true)
        app.MapGet("Lists", async (DBContext context, UserManager<Users> userManager, ClaimsPrincipal user, Matters? matter, bool? preList) =>
        {
            var userId = userManager.GetCurrentUserId(user);
            if (userId == null) return Results.Unauthorized();

            var isStaff = user.IsStaff();
            var query = context.Lists.Where(l => l.OwnerId == null);

            if (matter != null) query = query.Where(l => l.Matter == matter);
            if (preList != null) query = query.Where(l => l.PreList == preList);

            if (!isStaff)
            {
                var me = await userManager.FindByIdAsync(userId.Value.ToString());

                if (me?.Objective == null)
                {
                    query = query.Where(l => !l.Objectives.Any());
                }
                else
                {
                    var objective = me.Objective.Value;
                    query = query.Where(l => !l.Objectives.Any() || l.Objectives.Any(o => o.Objective == objective));
                }
            }

            var lists = await query
                .OrderBy(l => l.CreatedAt)
                .Select(l => new
                {
                    l.Id,
                    l.Title,
                    l.Description,
                    l.Matter,
                    l.PreList,
                    Questions = l.Questions.Count(),
                    Objectives = l.Objectives.Select(o => o.Objective)
                })
                .ToListAsync();

            return Results.Ok(lists);
        });

        // Listas personalizadas do aluno
        app.MapGet("Lists/Mine", async (DBContext context, UserManager<Users> userManager, ClaimsPrincipal user) =>
        {
            var userId = userManager.GetCurrentUserId(user);
            if (userId == null) return Results.Unauthorized();

            var lists = await context.Lists
                .Where(l => l.OwnerId == userId)
                .OrderByDescending(l => l.CreatedAt)
                .Select(l => new { l.Id, l.Title, l.Matter, Questions = l.Questions.Count(), l.CreatedAt })
                .ToListAsync();

            return Results.Ok(lists);
        });

        // Uma lista com as questões e alternativas
        app.MapGet("Lists/{id:int}", async (DBContext context, int id, UserManager<Users> userManager, ClaimsPrincipal user) =>
        {
            var userId = userManager.GetCurrentUserId(user);
            if (userId == null) return Results.Unauthorized();

            var isStaff = user.IsStaff();
            var list = await context.Lists.Include(l => l.Objectives).FirstOrDefaultAsync(l => l.Id == id);
            if (list == null) return Results.NotFound();

            var me = await userManager.FindByIdAsync(userId.Value.ToString());

            // Lista que a pessoa não pode ver responde 404, para não revelar que ela existe
            if (!CanView(list, userId.Value, isStaff, me?.Objective)) return Results.NotFound();

            var items = await LoadItemsAsync(context, id);
            return Results.Ok(ToDetail(list, items, isStaff));
        });

        // PDF da lista (gerado na hora, sempre com o conteúdo atual)
        app.MapGet("Lists/{id:int}/Pdf", async (DBContext context, int id, UserManager<Users> userManager, ClaimsPrincipal user, IHttpClientFactory httpFactory, IConfiguration config) =>
        {
            var userId = userManager.GetCurrentUserId(user);
            if (userId == null) return Results.Unauthorized();

            var list = await context.Lists.Include(l => l.Objectives).FirstOrDefaultAsync(l => l.Id == id);
            if (list == null) return Results.NotFound();

            var me = await userManager.FindByIdAsync(userId.Value.ToString());
            if (!CanView(list, userId.Value, user.IsStaff(), me?.Objective)) return Results.NotFound();

            var items = await LoadItemsAsync(context, id);
            if (items.Count == 0) return Results.BadRequest("This list has no questions.");

            var images = await ListPdf.DownloadImagesAsync(httpFactory.CreateClient(), items);

            var coverArt = ListPdf.LoadCoverArt(list.Matter);
            var resolutionUrl = config["App:ResolutionUrl"];   // ex: https://meusite.com/resolucao/{id}

            byte[] pdf;
            try
            {
                pdf = ListPdf.Generate(list, items, images, coverArt, resolutionUrl, sideBySide: true);
            }
            catch (Exception)
            {
                // Alguma questão não coube numa coluna: tenta de novo com 1 questão por página, em largura total
                pdf = ListPdf.Generate(list, items, images, coverArt, resolutionUrl, sideBySide: false);
            }

            return Results.File(pdf, "application/pdf", $"lista-{list.Id}.pdf");
        });

        // Criar lista oficial (administradores e funcionários)
        app.MapPost("Lists", async (DBContext context, ListsRequest req, UserManager<Users> userManager, ClaimsPrincipal user) =>
        {
            var userId = userManager.GetCurrentUserId(user);
            if (userId == null) return Results.Unauthorized();
            if (!user.IsStaff()) return Results.Forbid();

            var error = await ValidateAsync(context, req);
            if (error != null) return Results.BadRequest(error);

            var list = new Lists
            {
                Title = req.Title.Trim(),
                Description = req.Description?.Trim(),
                Matter = req.Matter,
                PreList = req.PreList,
                Questions = MakeQuestions(req),
                Objectives = MakeObjectives(req)
            };

            context.Lists.Add(list);
            await context.SaveChangesAsync();

            return Results.Ok(new { list.Id, list.Title, list.Matter, list.PreList, Questions = list.Questions.Count });
        });

        // Editar lista oficial: troca título, objetivos e a lista de questões (com a nova ordem)
        app.MapPut("Lists/{id:int}", async (DBContext context, int id, ListsRequest req, UserManager<Users> userManager, ClaimsPrincipal user) =>
        {
            var userId = userManager.GetCurrentUserId(user);
            if (userId == null) return Results.Unauthorized();
            if (!user.IsStaff()) return Results.Forbid();

            var list = await context.Lists.FirstOrDefaultAsync(l => l.Id == id && l.OwnerId == null);
            if (list == null) return Results.NotFound();

            var error = await ValidateAsync(context, req);
            if (error != null) return Results.BadRequest(error);

            // Apaga o conteúdo antigo e grava o novo, tudo na mesma transação
            await using var transaction = await context.Database.BeginTransactionAsync();

            await context.ListsQuestions.Where(x => x.ListId == id).ExecuteDeleteAsync();
            await context.ListsObjectives.Where(x => x.ListId == id).ExecuteDeleteAsync();

            list.Title = req.Title.Trim();
            list.Description = req.Description?.Trim();
            list.Matter = req.Matter;
            list.PreList = req.PreList;
            list.Questions.AddRange(MakeQuestions(req));
            list.Objectives.AddRange(MakeObjectives(req));

            await context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Results.Ok(new { list.Id, list.Title, list.Matter, list.PreList, Questions = req.QuestionIds.Count });
        });

        // Apagar: a equipe apaga listas oficiais, o aluno apaga as próprias
        app.MapDelete("Lists/{id:int}", async (DBContext context, int id, UserManager<Users> userManager, ClaimsPrincipal user) =>
        {
            var userId = userManager.GetCurrentUserId(user);
            if (userId == null) return Results.Unauthorized();

            var list = await context.Lists.FirstOrDefaultAsync(l => l.Id == id);
            if (list == null) return Results.NotFound();

            if (list.OwnerId == null)
            {
                if (!user.IsStaff()) return Results.Forbid();
            }
            else if (list.OwnerId != userId)
            {
                return Results.NotFound();
            }

            context.Lists.Remove(list);   // as questões da lista saem junto (cascata); as questões em si continuam
            await context.SaveChangesAsync();
            return Results.Ok();
        });

        // Lista personalizada do aluno: uma matéria, número de questões, dificuldades e assuntos
        app.MapPost("Lists/Custom", async (DBContext context, CustomListsRequest req, UserManager<Users> userManager, ClaimsPrincipal user) =>
        {
            var userId = userManager.GetCurrentUserId(user);
            if (userId == null) return Results.Unauthorized();

            if (!Enum.IsDefined(req.Matter))
                return Results.BadRequest("Invalid matter.");
            if (req.Count < 1 || req.Count > MaxCustomQuestions)
                return Results.BadRequest($"Count must be between 1 and {MaxCustomQuestions}.");
            if (req.Title is not null && req.Title.Length > 150)
                return Results.BadRequest("Title exceeds the maximum length of 150 characters.");

            var difficulties = req.Difficulties.Distinct().ToList();
            if (difficulties.Any(d => !Enum.IsDefined(d)))
                return Results.BadRequest("Invalid difficulty.");

            // Os assuntos precisam existir e ser da matéria escolhida (nada de misturar matérias)
            var topicIds = req.TopicIds.Distinct().ToList();
            if (topicIds.Count > 0)
            {
                var validTopics = await context.Topics
                    .CountAsync(t => topicIds.Contains(t.Id) && t.Branch.Matter == req.Matter);

                if (validTopics != topicIds.Count)
                    return Results.BadRequest("Topics must exist and belong to the chosen matter.");
            }

            var existing = await context.Lists.CountAsync(l => l.OwnerId == userId);
            if (existing >= MaxCustomListsPerUser)
                return Results.BadRequest($"You can have at most {MaxCustomListsPerUser} custom lists. Delete one first.");

            var query = context.Questions.Where(q => q.Topic.Branch.Matter == req.Matter);
            if (topicIds.Count > 0) query = query.Where(q => topicIds.Contains(q.TopicId));
            if (difficulties.Count > 0) query = query.Where(q => difficulties.Contains(q.Difficulty));

            var available = await query.CountAsync();
            if (available < req.Count)
                return Results.BadRequest(new { error = "Not enough questions match these filters.", available });

            // Sorteio feito no banco (NEWID)
            var questionIds = await query
                .OrderBy(q => Guid.NewGuid())
                .Take(req.Count)
                .Select(q => q.Id)
                .ToListAsync();

            var title = string.IsNullOrWhiteSpace(req.Title)
                ? $"Lista personalizada de {req.Matter}"
                : req.Title.Trim();

            var list = new Lists
            {
                Title = title,
                Matter = req.Matter,
                PreList = false,
                OwnerId = userId.Value,
                Questions = questionIds
                    .Select((questionId, index) => new ListsQuestions { QuestionId = questionId, Order = index + 1 })
                    .ToList()
            };

            context.Lists.Add(list);
            await context.SaveChangesAsync();

            return Results.Ok(new { list.Id, list.Title, list.Matter, Questions = questionIds.Count });
        });
    }
}
