using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using MyCourse.Data;
using MyCourse.Models;

namespace MyCourse.Routes;

public static class SupportsRoute
{
    public static void SupportsRoutes(this WebApplication app)
    {
        // Listar chamados do usuário logado
        app.MapGet("Supports", async (
            DBContext context,
            UserManager<Users> userManager,
            ClaimsPrincipal user) =>
        {
            var userId = userManager.GetCurrentUserId(user);

            if (userId == null)
                return Results.Unauthorized();

            var supports = await context.Supports
                .Where(s => s.UserId == userId)
                .Include(s => s.Subject)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();

            return Results.Ok(supports);
        });

        // Consultar um chamado específico
        app.MapGet("Supports/{id:guid}", async (
            DBContext context,
            Guid id,
            UserManager<Users> userManager,
            ClaimsPrincipal user) =>
        {
            var userId = userManager.GetCurrentUserId(user);

            if (userId == null)
                return Results.Unauthorized();

            var support = await context.Supports
                .Include(s => s.Subject)
                .FirstOrDefaultAsync(s =>
                    s.Id == id && s.UserId == userId);

            if (support == null)
                return Results.NotFound();

            return Results.Ok(support);
        });

        // Criar chamado
        app.MapPost("Supports", async (
            DBContext context,
            SupportsRequest req,
            UserManager<Users> userManager,
            ClaimsPrincipal user) =>
        {
            var userId = userManager.GetCurrentUserId(user);

            if (userId == null)
                return Results.Unauthorized();

            if (string.IsNullOrWhiteSpace(req.Text))
                return Results.BadRequest("Text cannot be empty.");

            if (req.Text.Length > 10000)
                return Results.BadRequest("Text exceeds 10,000 characters.");

            if (!await context.Subjects.AnyAsync(s => s.Id == req.SubjectId))
                return Results.NotFound("Subject not found.");

            var support = new Supports(
                req.SubjectId,
                req.Text,
                userId.Value
            );

            context.Supports.Add(support);

            await context.SaveChangesAsync();

            return Results.Created($"Supports/{support.Id}", support);
        });

        // Editar chamado
        app.MapPut("Supports/{id:guid}", async (
            DBContext context,
            Guid id,
            SupportsRequest req,
            UserManager<Users> userManager,
            ClaimsPrincipal user) =>
        {
            var userId = userManager.GetCurrentUserId(user);

            if (userId == null)
                return Results.Unauthorized();

            if (string.IsNullOrWhiteSpace(req.Text))
                return Results.BadRequest("Text cannot be empty.");

            if (req.Text.Length > 10000)
                return Results.BadRequest("Text exceeds 10,000 characters.");

            var support = await context.Supports
                .FirstOrDefaultAsync(s =>
                    s.Id == id && s.UserId == userId);

            if (support == null)
                return Results.NotFound();

            if (!await context.Subjects.AnyAsync(s => s.Id == req.SubjectId))
                return Results.NotFound("Subject not found.");

            support.SubjectId = req.SubjectId;
            support.Text = req.Text;

            await context.SaveChangesAsync();

            return Results.Ok(support);
        });

        // Excluir chamado
        app.MapDelete("Supports/{id:guid}", async (
            DBContext context,
            Guid id,
            UserManager<Users> userManager,
            ClaimsPrincipal user) =>
        {
            var userId = userManager.GetCurrentUserId(user);

            if (userId == null)
                return Results.Unauthorized();

            var support = await context.Supports
                .FirstOrDefaultAsync(s =>
                    s.Id == id && s.UserId == userId);

            if (support == null)
                return Results.NotFound();

            context.Supports.Remove(support);

            await context.SaveChangesAsync();

            return Results.Ok();
        });
    }
}