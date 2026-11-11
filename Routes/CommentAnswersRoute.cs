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
        app.MapGet("Comments/{Id=guid}/Answers", async (DBContext context, Guid id) =>
        {
            var answers = await context.CommentAnswers.Where(o => o.CommentId == id).ToListAsync();
            return Results.Ok(answers);
        });

        app.MapPost("Comments/{Id=guid}/Answers", async (DBContext context, Guid id, CommentAnswersRequests req) =>
        {
            if (req.Content.Length > 10000)
            {
                return Results.BadRequest("Content exceeds the maximum length of 10,000 characters.");
            }
            
            else if (string.IsNullOrWhiteSpace(req.Content))
            {
                return Results.BadRequest("Content cannot be empty.");
            }

            var commentAnswer = new CommentAnswers(req.CommentId, req.UserId, req.Content, req.ImageUrl ?? string.Empty);
            context.CommentAnswers.Add(commentAnswer);
            await context.SaveChangesAsync();
            return Results.Ok(commentAnswer);
        });

        app.MapPut("Lessons/{VideoId=guid}/Comments/{CommentId=guid}/Answers/{id=guid}", async (DBContext context, Guid id, CommentAnswersRequests req) =>
        {
            if (req.Content.Length > 10000)
            {
                return Results.BadRequest("Content exceeds the maximum length of 10,000 characters.");
            }

            else if (string.IsNullOrWhiteSpace(req.Content))
            {
                return Results.BadRequest("Content cannot be empty.");
            }

            var commentAnswer = await context.CommentAnswers.FindAsync(id);
            if (commentAnswer == null)
                return Results.NotFound();

            commentAnswer.Content = req.Content;
            commentAnswer.CommentId = req.CommentId;
            commentAnswer.ImageUrl = req.ImageUrl ?? string.Empty;

            await context.SaveChangesAsync();
            return Results.Ok(commentAnswer);
        });

        app.MapDelete("Lessons/{VideoId=guid}/Comments/{CommentId=guid}/Answers/{id=guid}", async (DBContext context, Guid videoId, Guid id, UserManager<Users> userManager, ClaimsPrincipal user) =>
        {
            var commentAnswer = await context.CommentAnswers.FindAsync(id);
            if (commentAnswer == null) return Results.NotFound();

            var userIdText = userManager.GetUserId(user);
            if (string.IsNullOrWhiteSpace(userIdText)) return Results.Unauthorized();

            var userId = Guid.Parse(userIdText);
            if (commentAnswer.UserId != userId) return Results.Forbid();

            context.CommentAnswers.Remove(commentAnswer);
            await context.SaveChangesAsync();
            return Results.Ok();
        });
    }
}