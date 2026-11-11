using Microsoft.EntityFrameworkCore;
using MyCourse.Data;
using MyCourse.Models;

namespace MyCourse.Routes;

public static class VideosRoute
{
    public static void VideosRoutes(this WebApplication app)
    {
        app.MapGet("Lessons/{Id=guid}", async (DBContext context, Guid id) =>
        {
            var video = await context.Videos.FindAsync(id);
            return Results.Ok(video);
        });

        app.MapGet("Lessons", async (DBContext context, Matters? mat) =>
        {
            if (mat == null)
            {
                return Results.BadRequest("Matter parameter is required.");
            }
            var videos = await context.Videos.Where(v => v.Topic.Branch.Matter == mat).ToListAsync();
            return Results.Ok(videos);
        });

        app.MapGet("Controlpanel/Lessons", async (DBContext context) =>
        {
            var videos = await context.Videos.ToListAsync();
            return Results.Ok(videos);
        });

        app.MapPost("Controlpanel/Lessons", async (DBContext context, VideosRequest req) =>
        {
            var video = new Videos(req.Title, req.Description ?? string.Empty, req.Url, req.TopicId);
            context.Videos.Add(video);
            await context.SaveChangesAsync();
            return Results.Ok(video);
        });

        app.MapPut("Controlpanel/Lessons/{Id=guid}", async (DBContext context, Guid id, VideosRequest req) =>
        {
            var video = await context.Videos.FindAsync(id);
            if (video == null)
                return Results.NotFound();

            video.Title = req.Title;
            video.Description = req.Description ?? string.Empty;
            video.Url = req.Url;
            video.TopicId = req.TopicId;

            await context.SaveChangesAsync();
            return Results.Ok(video);
        });

        app.MapDelete("Controlpanel/Lessons/{Id=guid}", async (DBContext context, Guid id) =>
        {
            var video = await context.Videos.FindAsync(id);
            if (video == null)
                return Results.NotFound();

            context.Videos.Remove(video);
            await context.SaveChangesAsync();
            return Results.Ok();
        });
    }
}