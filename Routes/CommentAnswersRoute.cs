using MyCourse.Data;
using MyCourse.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace MyCourse.Routes;

public static class CommentAnswersRoute
{
    public static void CommentAnswersRoutes(this WebApplication app)
    {
        app.MapGet("Lessons/{videoId:guid}/Comments/{id:guid}/Answers", async (DBContext context, Guid id) =>
        {
            var answers = await context.CommentAnswers.Where(o => o.CommentId == id).ToListAsync();
            return Results.Ok(answers);
        });

        app.MapGet("Comments/{id:guid}/Answers", async (DBContext context, Guid id) =>
        {
            var answers = await context.CommentAnswers.Where(o => o.CommentId == id).ToListAsync();
            return Results.Ok(answers);
        });

        app.MapPost("Lessons/{videoId:guid}/Comments/{id:guid}/Answers", async (DBContext context, Guid videoId, Guid id, CommentAnswersRequests req, UserManager<Users> userManager, ClaimsPrincipal user) =>
        {
            var userId = userManager.GetCurrentUserId(user);
            if (userId == null) return Results.Unauthorized();

            if (string.IsNullOrWhiteSpace(req.Content))
            {
                return Results.BadRequest("Content cannot be empty.");
            }

            else if (req.Content.Length > 10000)
            {
                return Results.BadRequest("Content exceeds the maximum length of 10,000 characters.");
            }

            if (!await context.Comments.AnyAsync(c => c.Id == id && c.VideoId == videoId))
                return Results.NotFound();

            var commentAnswer = new CommentAnswers(id, userId.Value, req.Content, req.ImageUrl ?? string.Empty);
            context.CommentAnswers.Add(commentAnswer);
            await context.SaveChangesAsync();
            return Results.Ok(commentAnswer);
        }).RequireAuthorization(o => o.RequireRole("Adm", "Staff", "Support", "Studant"));

        app.MapPut("Lessons/{videoId:guid}/Comments/{commentId:guid}/Answers/{id:guid}", async (DBContext context, Guid commentId, Guid id, CommentAnswersRequests req, UserManager<Users> userManager, ClaimsPrincipal user) =>
        {
            var userId = userManager.GetCurrentUserId(user);
            if (userId == null) return Results.Unauthorized();

            if (string.IsNullOrWhiteSpace(req.Content))
            {
                return Results.BadRequest("Content cannot be empty.");
            }

            else if (req.Content.Length > 10000)
            {
                return Results.BadRequest("Content exceeds the maximum length of 10,000 characters.");
            }

            var commentAnswer = await context.CommentAnswers.FirstOrDefaultAsync(a => a.Id == id && a.CommentId == commentId);
            if (commentAnswer == null)
                return Results.NotFound();

            if (commentAnswer.UserId != userId) return Results.Forbid();

            commentAnswer.Content = req.Content;
            commentAnswer.ImageUrl = req.ImageUrl ?? string.Empty;

            await context.SaveChangesAsync();
            return Results.Ok(commentAnswer);
        }).RequireAuthorization(o => o.RequireRole("Adm", "Staff", "Support", "Studant"));

        app.MapDelete("Lessons/{videoId:guid}/Comments/{commentId:guid}/Answers/{id:guid}", async (DBContext context, Guid commentId, Guid id, UserManager<Users> userManager, ClaimsPrincipal user) =>
        {
            var userId = userManager.GetCurrentUserId(user);
            if (userId == null) return Results.Unauthorized();

            var commentAnswer = await context.CommentAnswers.FirstOrDefaultAsync(a => a.Id == id && a.CommentId == commentId);
            if (commentAnswer == null) return Results.NotFound();

            if (commentAnswer.UserId != userId) return Results.Forbid();

            context.CommentAnswers.Remove(commentAnswer);
            await context.SaveChangesAsync();
            return Results.Ok();
        }).RequireAuthorization(o => o.RequireRole("Adm", "Staff", "Support", "Studant"));
    }
}
