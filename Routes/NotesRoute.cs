using Microsoft.EntityFrameworkCore;
using MyCourse.Data;
using MyCourse.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace MyCourse.Routes;

public static class NotesRoute
{
    // Devolve a mensagem de erro, ou null se estiver tudo certo
    private static string? Validate(NotesRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Title))
            return "Title cannot be empty.";

        if (req.Title.Length > 100)
            return "Title exceeds the maximum length of 100 characters.";

        if (req.Description is not null && req.Description.Length > 10000)
            return "Description exceeds the maximum length of 10,000 characters.";

        if (!Enum.IsDefined(req.Color))
            return "Invalid color.";

        return null;
    }

    public static void NotesRoutes(this WebApplication app)
    {
        // Todas as notas do aluno (filtro opcional: Notes?color=Blue)
        app.MapGet("Notes", async (DBContext context, UserManager<Users> userManager, ClaimsPrincipal user, ColorNotes? color) =>
        {
            var userId = userManager.GetCurrentUserId(user);
            if (userId == null) return Results.Unauthorized();

            var query = context.Notes.Where(n => n.UserId == userId);
            if (color != null)
                query = query.Where(n => n.Color == color);

            var notes = await query
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new { n.Id, n.Title, n.Description, n.Color, n.VideoId, VideoTitle = n.Video.Title, n.CreatedAt })
                .ToListAsync();

            return Results.Ok(notes);
        });

        // Resumo para as barrinhas coloridas: cores e quantidades por aula, numa chamada só
        app.MapGet("Notes/Summary", async (DBContext context, UserManager<Users> userManager, ClaimsPrincipal user) =>
        {
            var userId = userManager.GetCurrentUserId(user);
            if (userId == null) return Results.Unauthorized();

            var rows = await context.Notes
                .Where(n => n.UserId == userId)
                .GroupBy(n => new { n.VideoId, n.Color })
                .Select(g => new { g.Key.VideoId, g.Key.Color, Count = g.Count() })
                .ToListAsync();

            var summary = rows
                .GroupBy(r => r.VideoId)
                .Select(g => new
                {
                    VideoId = g.Key,
                    Colors = g.Select(x => new { x.Color, x.Count })
                });

            return Results.Ok(summary);
        });

        // Uma nota
        app.MapGet("Notes/{id:guid}", async (DBContext context, Guid id, UserManager<Users> userManager, ClaimsPrincipal user) =>
        {
            var userId = userManager.GetCurrentUserId(user);
            if (userId == null) return Results.Unauthorized();

            var note = await context.Notes
                .Where(n => n.Id == id && n.UserId == userId)
                .Select(n => new { n.Id, n.Title, n.Description, n.Color, n.VideoId, VideoTitle = n.Video.Title, n.CreatedAt })
                .FirstOrDefaultAsync();

            if (note == null) return Results.NotFound();
            return Results.Ok(note);
        });

        // Notas do aluno em uma aula
        app.MapGet("Lessons/{id:guid}/Notes", async (DBContext context, Guid id, UserManager<Users> userManager, ClaimsPrincipal user) =>
        {
            var userId = userManager.GetCurrentUserId(user);
            if (userId == null) return Results.Unauthorized();

            var notes = await context.Notes
                .Where(n => n.VideoId == id && n.UserId == userId)
                .OrderBy(n => n.CreatedAt)
                .Select(n => new { n.Id, n.Title, n.Description, n.Color, n.VideoId, n.CreatedAt })
                .ToListAsync();

            return Results.Ok(notes);
        });

        // Criar nota em uma aula
        app.MapPost("Lessons/{id:guid}/Notes", async (DBContext context, Guid id, NotesRequest req, UserManager<Users> userManager, ClaimsPrincipal user) =>
        {
            var userId = userManager.GetCurrentUserId(user);
            if (userId == null) return Results.Unauthorized();

            var error = Validate(req);
            if (error != null) return Results.BadRequest(error);

            if (!await context.Videos.AnyAsync(v => v.Id == id))
                return Results.NotFound();

            var note = new Notes();
            note.NewNote(req.Title.Trim(), req.Description, req.Color, id, userId.Value);

            context.Notes.Add(note);
            await context.SaveChangesAsync();

            return Results.Ok(new { note.Id, note.Title, note.Description, note.Color, note.VideoId, note.CreatedAt });
        });

        // Editar nota (título, descrição e cor)
        app.MapPut("Lessons/{videoId:guid}/Notes/{id:guid}", async (DBContext context, Guid videoId, Guid id, NotesRequest req, UserManager<Users> userManager, ClaimsPrincipal user) =>
        {
            var userId = userManager.GetCurrentUserId(user);
            if (userId == null) return Results.Unauthorized();

            var error = Validate(req);
            if (error != null) return Results.BadRequest(error);

            // Nota de outro aluno responde 404, para não revelar que ela existe
            var note = await context.Notes.FirstOrDefaultAsync(n => n.Id == id && n.VideoId == videoId && n.UserId == userId);
            if (note == null) return Results.NotFound();

            note.Title = req.Title.Trim();
            note.Description = req.Description;
            note.Color = req.Color;

            await context.SaveChangesAsync();
            return Results.Ok(new { note.Id, note.Title, note.Description, note.Color, note.VideoId, note.CreatedAt });
        });

        // Apagar nota
        app.MapDelete("Lessons/{videoId:guid}/Notes/{id:guid}", async (DBContext context, Guid videoId, Guid id, UserManager<Users> userManager, ClaimsPrincipal user) =>
        {
            var userId = userManager.GetCurrentUserId(user);
            if (userId == null) return Results.Unauthorized();

            var note = await context.Notes.FirstOrDefaultAsync(n => n.Id == id && n.VideoId == videoId && n.UserId == userId);
            if (note == null) return Results.NotFound();

            context.Notes.Remove(note);
            await context.SaveChangesAsync();
            return Results.Ok();
        });
    }
}
