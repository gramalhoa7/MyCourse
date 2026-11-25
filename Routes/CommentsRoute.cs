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
            var userId = userManager.GetCurrentUserId(user);
            if (userId == null) return Results.Unauthorized();

            var comments = await context.Comments.Where(c => c.UserId == userId).ToListAsync();
            return Results.Ok(comments);
        });
        
        app.MapGet("Lessons/{id:guid}/Comments", async (DBContext context, Guid id) =>
        {
            var comments = await context.Comments.Where(c => c.VideoId == id).ToListAsync();
            return Results.Ok(comments);
        });

        app.MapPost("Lessons/{id:guid}/Comments", async (DBContext context, Guid id, CommentsRequest req, UserManager<Users> userManager, ClaimsPrincipal user) =>
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

            if (!await context.Videos.AnyAsync(v => v.Id == id))
                return Results.NotFound();

            var comment = new Comments(id, userId.Value, req.Content, req.ImageUrl ?? string.Empty);
            context.Comments.Add(comment);
            await context.SaveChangesAsync();
            return Results.Ok(comment);
        }).RequireAuthorization(o => o.RequireRole("Adm", "Staff", "Support", "Studant"));

        app.MapPut("Lessons/{videoId:guid}/Comments/{id:guid}", async (DBContext context, Guid videoId, Guid id, CommentsRequest req, UserManager<Users> userManager, ClaimsPrincipal user) =>
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

            var comment = await context.Comments.FirstOrDefaultAsync(c => c.Id == id && c.VideoId == videoId);
            if (comment == null)
                return Results.NotFound();

            if (comment.UserId != userId) return Results.Forbid();

            comment.Content = req.Content;
            comment.ImageUrl = req.ImageUrl ?? string.Empty;

            await context.SaveChangesAsync();
            return Results.Ok(comment);
        }).RequireAuthorization(o => o.RequireRole("Adm", "Staff", "Support", "Studant"));

        app.MapDelete("Lessons/{videoId:guid}/Comments/{id:guid}", async (DBContext context, Guid videoId, Guid id, UserManager<Users> userManager, ClaimsPrincipal user) =>
        {
            var userId = userManager.GetCurrentUserId(user);
            if (userId == null) return Results.Unauthorized();

            var comment = await context.Comments.FirstOrDefaultAsync(c => c.Id == id && c.VideoId == videoId);
            if (comment == null) return Results.NotFound();

            if (comment.UserId != userId) return Results.Forbid();

            context.Comments.Remove(comment);
            await context.SaveChangesAsync();
            return Results.Ok();
        }).RequireAuthorization(o => o.RequireRole("Adm", "Staff", "Support", "Studant"));
    }
}
