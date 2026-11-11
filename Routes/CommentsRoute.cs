using Microsoft.EntityFrameworkCore;
using MyCourse.Data;
using MyCourse.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace MyCourse.Routes;

public static class CommentsRoute
{
    public static void CommentsRoutes(this WebApplication app)
    {
        app.MapGet("Comments", async (DBContext context, UserManager<Users> userManager, ClaimsPrincipal user) =>
        {
            var userIdText = userManager.GetUserId(user);
            if (string.IsNullOrWhiteSpace(userIdText)) return Results.Unauthorized();

            var userId = Guid.Parse(userIdText);
            var comments = await context.Comments.Where(c => c.UserId == userId).ToListAsync();
            return Results.Ok(comments);
        });
        
        app.MapGet("Lessons/{Id=guid}/Comments", async (DBContext context, Guid id) =>
        {
            var comments = await context.Comments.Where(c => c.VideoId == id).ToListAsync();
            return Results.Ok(comments);
        });

        app.MapPost("Lessons/{Id=guid}/Comments", async (DBContext context, Guid id, CommentsRequest req) =>
        {
            if (req.Content.Length > 10000)
            {
                return Results.BadRequest("Content exceeds the maximum length of 10,000 characters.");
            }
            
            else if (string.IsNullOrWhiteSpace(req.Content))
            {
                return Results.BadRequest("Content cannot be empty.");
            }

            var comment = new Comments(req.VideoId, req.UserId, req.Content, req.ImageUrl ?? string.Empty);
            context.Comments.Add(comment);
            await context.SaveChangesAsync();
            return Results.Ok(comment);
        });

        app.MapPut("Lessons/{VideoId=guid}/Comments/{id=guid}", async (DBContext context, Guid id, CommentsRequest req) =>
        {
            if (req.Content.Length > 10000)
            {
                return Results.BadRequest("Content exceeds the maximum length of 10,000 characters.");
            }

            else if (string.IsNullOrWhiteSpace(req.Content))
            {
                return Results.BadRequest("Content cannot be empty.");
            }

            var comment = await context.Comments.FindAsync(id);
            if (comment == null)
                return Results.NotFound();

            comment.Content = req.Content;
            comment.VideoId = req.VideoId;
            comment.ImageUrl = req.ImageUrl ?? string.Empty;

            await context.SaveChangesAsync();
            return Results.Ok(comment);
        });

        app.MapDelete("Lessons/{VideoId=guid}/Comments/{Id=guid}", async (DBContext context, Guid videoId, Guid id, UserManager<Users> userManager, ClaimsPrincipal user) =>
        {
            var comment = await context.Comments.FindAsync(id);
            if (comment == null) return Results.NotFound();

            var userIdText = userManager.GetUserId(user);
            if (string.IsNullOrWhiteSpace(userIdText))
                return Results.Unauthorized();

            var userId = Guid.Parse(userIdText);
            if (comment.UserId != userId) return Results.Forbid();

            context.Comments.Remove(comment);
            await context.SaveChangesAsync();
            return Results.Ok();
        });
    }
}