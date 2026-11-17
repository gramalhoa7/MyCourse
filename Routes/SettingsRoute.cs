using Microsoft.EntityFrameworkCore;
using MyCourse.Data;
using MyCourse.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace MyCourse.Routes;

public static class SettingsRoute
{
    // Busca as configurações do aluno; se ele ainda não tem, cria com os valores padrão
    private static async Task<UserSettings> GetOrCreateAsync(DBContext context, Guid userId)
    {
        var settings = await context.Settings.FirstOrDefaultAsync(s => s.UserId == userId);
        if (settings != null) return settings;

        settings = new UserSettings { UserId = userId };
        settings.SetDefaultSettings();
        context.Settings.Add(settings);

        try
        {
            await context.SaveChangesAsync();
            return settings;
        }
        catch (DbUpdateException)
        {
            // Duas chamadas criaram ao mesmo tempo e a outra venceu: é só ler a linha dela
            context.Entry(settings).State = EntityState.Detached;
            return await context.Settings.FirstAsync(s => s.UserId == userId);
        }
    }

    private static object ToResponse(UserSettings s) => new
    {
        s.DarkTheme,
        s.EmailNotifications,
        s.AutoPlayNextVideo,
        s.TextSize,
        s.HighContrast,
        s.ReduceMotion
    };

    public static void SettingsRoutes(this WebApplication app)
    {
        // Todas as configurações de quem está logado
        app.MapGet("Settings", async (DBContext context, UserManager<Users> userManager, ClaimsPrincipal user) =>
        {
            var userId = userManager.GetCurrentUserId(user);
            if (userId == null) return Results.Unauthorized();

            var settings = await GetOrCreateAsync(context, userId.Value);
            return Results.Ok(ToResponse(settings));
        });

        // Alterar (só o que vier preenchido). Serve também para importar as
        // configurações que a pessoa já tinha no navegador logo depois de criar a conta.
        app.MapPatch("Settings", async (DBContext context, SettingsRequest req, UserManager<Users> userManager, ClaimsPrincipal user) =>
        {
            var userId = userManager.GetCurrentUserId(user);
            if (userId == null) return Results.Unauthorized();

            if (req.TextSize != null && !Enum.IsDefined(req.TextSize.Value))
            {
                return Results.BadRequest("Invalid text size.");
            }

            var settings = await GetOrCreateAsync(context, userId.Value);

            if (req.DarkTheme != null) settings.DarkTheme = req.DarkTheme.Value;
            if (req.EmailNotifications != null) settings.EmailNotifications = req.EmailNotifications.Value;
            if (req.AutoPlayNextVideo != null) settings.AutoPlayNextVideo = req.AutoPlayNextVideo.Value;
            if (req.TextSize != null) settings.TextSize = req.TextSize.Value;
            if (req.HighContrast != null) settings.HighContrast = req.HighContrast.Value;
            if (req.ReduceMotion != null) settings.ReduceMotion = req.ReduceMotion.Value;

            await context.SaveChangesAsync();
            return Results.Ok(ToResponse(settings));
        });

        // Voltar tudo para o padrão
        app.MapPost("Settings/Reset", async (DBContext context, UserManager<Users> userManager, ClaimsPrincipal user) =>
        {
            var userId = userManager.GetCurrentUserId(user);
            if (userId == null) return Results.Unauthorized();

            var settings = await GetOrCreateAsync(context, userId.Value);
            settings.SetDefaultSettings();

            await context.SaveChangesAsync();
            return Results.Ok(ToResponse(settings));
        });
    }
}