using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using MyCourse.Data;
using MyCourse.Models;

namespace MyCourse.Routes;

public static class SupportAnswersRoute
{
    public static void SupportAnswersRoutes(this WebApplication app)
    {
        // Listar respostas de um chamado
        app.MapGet("Supports/{id:guid}/Answers", async (
            DBContext context,
            Guid id,
            UserManager<Users> userManager,
            ClaimsPrincipal user) =>
        {
            var userId = userManager.GetCurrentUserId(user);

            if (userId == null)
                return Results.Unauthorized();

            var support = await context.Supports
                .FirstOrDefaultAsync(s => s.Id == id);

            if (support == null)
                return Results.NotFound("Support not found.");

            bool isStaff = user.IsInRole("Support")
                        || user.IsInRole("Admin");

            if (support.UserId != userId && !isStaff)
                return Results.Forbid();

            var answers = await context.SupportAnswers
                .Where(a => a.SupportId == id)
                .OrderBy(a => a.CreatedAt)
                .ToListAsync();

            return Results.Ok(answers);
        });

        // Responder chamado (somente equipe de suporte)
        app.MapPost("Supports/{id:guid}/Answers", async (
            DBContext context,
            Guid id,
            SupportAnswersRequest req,
            UserManager<Users> userManager,
            ClaimsPrincipal user) =>
        {
            var userId = userManager.GetCurrentUserId(user);

            if (userId == null)
                return Results.Unauthorized();

            if (!user.IsInRole("Support") &&
                !user.IsInRole("Admin"))
                return Results.Forbid();

            if (string.IsNullOrWhiteSpace(req.Text))
                return Results.BadRequest("Text cannot be empty.");

            if (req.Text.Length > 10000)
                return Results.BadRequest("Text exceeds 10,000 characters.");

            if (!await context.Supports.AnyAsync(s => s.Id == id))
                return Results.NotFound("Support not found.");

            var answer = new SupportAnswers(
                id,
                userId.Value,
                req.Text
            );

            context.SupportAnswers.Add(answer);

            await context.SaveChangesAsync();

            return Results.Created(
                $"Supports/{id}/Answers/{answer.Id}",
                answer
            );
        }).RequireAuthorization(o => o.RequireRole("Adm", "Staff", "Support"));

        // Editar resposta
        app.MapPut("Supports/{id:guid}/Answers/{answerId:guid}", async (
            DBContext context,
            Guid id,
            Guid answerId,
            SupportAnswersRequest req,
            UserManager<Users> userManager,
            ClaimsPrincipal user) =>
        {
            var userId = userManager.GetCurrentUserId(user);

            if (userId == null)
                return Results.Unauthorized();

            if (!user.IsInRole("Support") &&
                !user.IsInRole("Admin"))
                return Results.Forbid();

            if (string.IsNullOrWhiteSpace(req.Text))
                return Results.BadRequest("Text cannot be empty.");

            if (req.Text.Length > 10000)
                return Results.BadRequest("Text exceeds 10,000 characters.");

            var answer = await context.SupportAnswers
                .FirstOrDefaultAsync(a =>
                    a.Id == answerId && a.SupportId == id);

            if (answer == null)
                return Results.NotFound();

            if (answer.UserId != userId)
                return Results.Forbid();

            answer.Text = req.Text;

            await context.SaveChangesAsync();

            return Results.Ok(answer);
        }).RequireAuthorization(o => o.RequireRole("Adm", "Staff", "Support"));

        // Excluir resposta
        app.MapDelete("Supports/{id:guid}/Answers/{answerId:guid}", async (
            DBContext context,
            Guid id,
            Guid answerId,
            UserManager<Users> userManager,
            ClaimsPrincipal user) =>
        {
            var userId = userManager.GetCurrentUserId(user);

            if (userId == null)
                return Results.Unauthorized();

            if (!user.IsInRole("Support") &&
                !user.IsInRole("Admin"))
                return Results.Forbid();

            var answer = await context.SupportAnswers
                .FirstOrDefaultAsync(a =>
                    a.Id == answerId && a.SupportId == id);

            if (answer == null)
                return Results.NotFound();

            if (answer.UserId != userId)
                return Results.Forbid();

            context.SupportAnswers.Remove(answer);

            await context.SaveChangesAsync();

            return Results.Ok();
        }).RequireAuthorization(o => o.RequireRole("Adm", "Staff", "Support"));
    }
}