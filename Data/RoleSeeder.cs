using Microsoft.AspNetCore.Identity;
using MyCourse.Models;

namespace MyCourse.Data;

public static class RoleSeeder
{
    // Cria os papéis Admin e Staff e, se existir "Seed:AdminEmail" na configuração,
    // torna essa conta administradora (a conta precisa já estar cadastrada).
    public static async Task SeedAsync(IServiceProvider services, IConfiguration config)
    {
        using var scope = services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("RoleSeeder");

        try
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Users>>();

            foreach (var role in new[] { Roles.Admin, Roles.Staff })
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }

            var adminEmail = config["Seed:AdminEmail"];
            if (string.IsNullOrWhiteSpace(adminEmail)) return;

            var admin = await userManager.FindByEmailAsync(adminEmail);
            if (admin == null)
            {
                logger.LogWarning("Seed:AdminEmail {Email} não está cadastrado ainda.", adminEmail);
                return;
            }

            if (!await userManager.IsInRoleAsync(admin, Roles.Admin))
                await userManager.AddToRoleAsync(admin, Roles.Admin);
        }
        catch (Exception ex)
        {
            // Acontece se o banco ainda não foi criado (antes do "database update")
            logger.LogWarning(ex, "Não foi possível criar os papéis agora.");
        }
    }
}
