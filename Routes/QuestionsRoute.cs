using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using MyCourse.Data;
using MyCourse.Models;

namespace MyCourse.Routes;

public static class QuestionsRoute
{
    private const int MaxAlternatives = 10;

    // ---------- Funções auxiliares ----------

    private static bool IsHttpUrl(string? url) =>
        Uri.TryCreate(url, UriKind.Absolute, out var uri) &&
        (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);

    private static bool BadUrl(string? url) =>
        !string.IsNullOrWhiteSpace(url) && (url.Length > 2000 || !IsHttpUrl(url));

    // Devolve a mensagem de erro, ou null se estiver tudo certo
    private static async Task<string?> ValidateAsync(DBContext context, QuestionsRequest req)
    {
        if (!Enum.IsDefined(req.Difficulty))
            return "Invalid difficulty.";
        if (string.IsNullOrWhiteSpace(req.Content) && string.IsNullOrWhiteSpace(req.ImageUrl))
            return "The question needs a text or an image.";
        if (req.Content is not null && req.Content.Length > 10000)
            return "Content exceeds the maximum length of 10,000 characters.";
        if (req.ResolutionText is not null && req.ResolutionText.Length > 20000)
            return "Resolution text exceeds the maximum length of 20,000 characters.";

        if (BadUrl(req.ImageUrl) || BadUrl(req.ResolutionImageUrl) || BadUrl(req.ResolutionVideoUrl))
            return "Image and video links must be complete http(s) addresses.";

        if (!await context.Topics.AnyAsync(t => t.Id == req.TopicId))
            return "Topic does not exist.";

        // Sem alternativas = questão discursiva (ITA/IME, por exemplo)
        if (req.Alternatives.Count == 0) return null;

        if (req.Alternatives.Count < 2 || req.Alternatives.Count > MaxAlternatives)
            return $"Use between 2 and {MaxAlternatives} alternatives, or none for a written-answer question.";

        var letters = req.Alternatives.Select(a => char.ToUpperInvariant(a.Letter)).ToList();
        if (letters.Any(l => l < 'A' || l > 'Z'))
            return "Alternative letters must be between A and Z.";
        if (letters.Distinct().Count() != letters.Count)
            return "Alternative letters cannot repeat.";
        if (req.Alternatives.Count(a => a.IsCorrect) != 1)
            return "Exactly one alternative must be correct.";

        foreach (var a in req.Alternatives)
        {
            if (string.IsNullOrWhiteSpace(a.Content) && string.IsNullOrWhiteSpace(a.ImageUrl))
                return "Each alternative needs a text or an image.";
            if (a.Content is not null && a.Content.Length > 2000)
                return "Alternative text exceeds the maximum length of 2,000 characters.";
            if (BadUrl(a.ImageUrl))
                return "Alternative image links must be complete http(s) addresses.";
        }

        return null;
    }

    private static void ApplyAlternative(Alternatives target, AlternativesRequest source)
    {
        target.Letter = char.ToUpperInvariant(source.Letter);
        target.Content = source.Content?.Trim() ?? string.Empty;
        target.ImageUrl = source.ImageUrl?.Trim() ?? string.Empty;
        target.IsCorrect = source.IsCorrect;
    }

    private static void ApplyQuestion(Questions target, QuestionsRequest source)
    {
        target.Content = source.Content?.Trim() ?? string.Empty;
        target.ImageUrl = source.ImageUrl?.Trim() ?? string.Empty;
        target.TopicId = source.TopicId;
        target.Difficulty = source.Difficulty;
        target.ResolutionText = string.IsNullOrWhiteSpace(source.ResolutionText) ? null : source.ResolutionText.Trim();
        target.ResolutionImageUrl = string.IsNullOrWhiteSpace(source.ResolutionImageUrl) ? null : source.ResolutionImageUrl.Trim();
        target.ResolutionVideoUrl = string.IsNullOrWhiteSpace(source.ResolutionVideoUrl) ? null : source.ResolutionVideoUrl.Trim();
    }

    // ---------- Rotas ----------

    public static void QuestionsRoutes(this WebApplication app)
    {
        // Assuntos de cada área, para montar os filtros (Topics?matter=Matemática)
        app.MapGet("Topics", async (DBContext context, Matters? matter) =>
        {
            var query = context.Branches.AsQueryable();
            if (matter != null) query = query.Where(b => b.Matter == matter);

            var branches = await query
                .OrderBy(b => b.Id)
                .Select(b => new
                {
                    BranchId = b.Id,
                    Branch = b.Name,
                    b.Matter,
                    Topics = b.Topic.OrderBy(t => t.Id).Select(t => new { t.Id, t.Name })
                })
                .ToListAsync();

            return Results.Ok(branches);
        });

        // Banco de questões para a equipe (com filtros e páginas)
        app.MapGet("Questions", async (DBContext context, UserManager<Users> userManager, ClaimsPrincipal user,
            Matters? matter, int? topicId, QuestionsDifficulty? difficulty, int? page, int? pageSize) =>
        {
            var userId = userManager.GetCurrentUserId(user);
            if (userId == null) return Results.Unauthorized();
            if (!user.IsStaff()) return Results.Forbid();

            var query = context.Questions.AsQueryable();
            if (matter != null) query = query.Where(q => q.Topic.Branch.Matter == matter);
            if (topicId != null) query = query.Where(q => q.TopicId == topicId);
            if (difficulty != null) query = query.Where(q => q.Difficulty == difficulty);

            var size = Math.Clamp(pageSize ?? 20, 1, 100);
            var current = Math.Max(page ?? 1, 1);
            var total = await query.CountAsync();

            var items = await query
                .OrderBy(q => q.Id)
                .Skip((current - 1) * size)
                .Take(size)
                .Select(q => new
                {
                    q.Id,
                    q.Content,
                    q.ImageUrl,
                    q.TopicId,
                    Topic = q.Topic.Name,
                    Matter = q.Topic.Branch.Matter,
                    q.Difficulty,
                    Alternatives = q.Alternatives.Count()
                })
                .ToListAsync();

            return Results.Ok(new { total, page = current, pageSize = size, items });
        });

        // Uma questão completa, com gabarito (para a tela de edição da equipe)
        app.MapGet("Questions/{id:int}", async (DBContext context, int id, UserManager<Users> userManager, ClaimsPrincipal user) =>
        {
            var userId = userManager.GetCurrentUserId(user);
            if (userId == null) return Results.Unauthorized();
            if (!user.IsStaff()) return Results.Forbid();

            var q = await context.Questions
                .Include(x => x.Topic).ThenInclude(t => t.Branch)
                .Include(x => x.Alternatives)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (q == null) return Results.NotFound();

            return Results.Ok(new
            {
                q.Id,
                q.Content,
                q.ImageUrl,
                q.TopicId,
                Topic = q.Topic.Name,
                Matter = q.Topic.Branch.Matter,
                q.Difficulty,
                q.ResolutionText,
                q.ResolutionImageUrl,
                q.ResolutionVideoUrl,
                Alternatives = q.Alternatives
                    .OrderBy(a => a.Letter)
                    .Select(a => new { a.Id, a.Letter, a.Content, a.ImageUrl, a.IsCorrect })
            });
        });

        // Resolução da questão: é para onde os links do gabarito do PDF levam.
        // Qualquer pessoa logada. (Quando existir o histórico de respostas, vale exigir que o aluno já tenha respondido.)
        app.MapGet("Questions/{id:int}/Resolution", async (DBContext context, int id, UserManager<Users> userManager, ClaimsPrincipal user) =>
        {
            var userId = userManager.GetCurrentUserId(user);
            if (userId == null) return Results.Unauthorized();

            var resolution = await context.Questions
                .Where(q => q.Id == id)
                .Select(q => new
                {
                    q.Id,
                    q.Content,
                    q.ImageUrl,
                    Topic = q.Topic.Name,
                    Matter = q.Topic.Branch.Matter,
                    q.Difficulty,
                    CorrectLetter = q.Alternatives.Where(a => a.IsCorrect).Select(a => (char?)a.Letter).FirstOrDefault(),
                    q.ResolutionText,
                    q.ResolutionImageUrl,
                    q.ResolutionVideoUrl
                })
                .FirstOrDefaultAsync();

            if (resolution == null) return Results.NotFound();
            return Results.Ok(resolution);
        });

        // Criar questão com as alternativas numa chamada só
        app.MapPost("Questions", async (DBContext context, QuestionsRequest req, UserManager<Users> userManager, ClaimsPrincipal user) =>
        {
            var userId = userManager.GetCurrentUserId(user);
            if (userId == null) return Results.Unauthorized();
            if (!user.IsStaff()) return Results.Forbid();

            var error = await ValidateAsync(context, req);
            if (error != null) return Results.BadRequest(error);

            var question = new Questions();
            ApplyQuestion(question, req);

            foreach (var a in req.Alternatives)
            {
                var alternative = new Alternatives();
                ApplyAlternative(alternative, a);
                question.Alternatives.Add(alternative);
            }

            context.Questions.Add(question);
            await context.SaveChangesAsync();

            return Results.Ok(new { question.Id });
        });

        // Editar questão. Alternativas de letras que continuam existindo são atualizadas (mantêm o Id);
        // as que sumiram são apagadas e as novas são criadas.
        app.MapPut("Questions/{id:int}", async (DBContext context, int id, QuestionsRequest req, UserManager<Users> userManager, ClaimsPrincipal user) =>
        {
            var userId = userManager.GetCurrentUserId(user);
            if (userId == null) return Results.Unauthorized();
            if (!user.IsStaff()) return Results.Forbid();

            var question = await context.Questions
                .Include(q => q.Alternatives)
                .FirstOrDefaultAsync(q => q.Id == id);
            if (question == null) return Results.NotFound();

            var error = await ValidateAsync(context, req);
            if (error != null) return Results.BadRequest(error);

            ApplyQuestion(question, req);

            var incoming = req.Alternatives.ToDictionary(a => char.ToUpperInvariant(a.Letter));

            foreach (var existing in question.Alternatives.ToList())
            {
                if (incoming.TryGetValue(existing.Letter, out var source))
                {
                    ApplyAlternative(existing, source);
                    incoming.Remove(existing.Letter);
                }
                else
                {
                    context.Alternatives.Remove(existing);
                }
            }

            foreach (var source in incoming.Values)
            {
                var alternative = new Alternatives();
                ApplyAlternative(alternative, source);
                question.Alternatives.Add(alternative);
            }

            await context.SaveChangesAsync();
            return Results.Ok(new { question.Id });
        });

        // Apagar questão (não deixa apagar se ela estiver em alguma lista)
        app.MapDelete("Questions/{id:int}", async (DBContext context, int id, UserManager<Users> userManager, ClaimsPrincipal user) =>
        {
            var userId = userManager.GetCurrentUserId(user);
            if (userId == null) return Results.Unauthorized();
            if (!user.IsStaff()) return Results.Forbid();

            var question = await context.Questions.FirstOrDefaultAsync(q => q.Id == id);
            if (question == null) return Results.NotFound();

            if (await context.ListsQuestions.AnyAsync(lq => lq.QuestionId == id))
                return Results.Conflict("This question is used in a list. Remove it from the lists first.");

            context.Questions.Remove(question);   // as alternativas saem junto (cascata)
            await context.SaveChangesAsync();
            return Results.Ok();
        });
    }
}
