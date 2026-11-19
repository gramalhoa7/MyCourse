using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyCourse.Data;
using MyCourse.Models;

namespace MyCourse.Routes;

public static class PlansRoute
{
    // Só um plano pode estar em destaque: desmarca todos os outros
    private static async Task ClearOtherHighlightsAsync(DBContext context, int exceptId)
    {
        var others = await context.Plans
            .Where(p => p.Highlight == true && p.Id != exceptId)
            .ToListAsync();

        foreach (var other in others)
            other.Highlight = false;
    }

    public static void PlansRoutes(this WebApplication app)
    {
        app.MapGet("Plans", async (DBContext context) =>
        {
            return Results.Ok(await context.Plans.Where(i => i.Status == true).OrderBy(o => o.Order).ToListAsync());
        });

        app.MapGet("AllPlans", async (DBContext context) =>
        {
            return Results.Ok(await context.Plans.ToListAsync());
        });

        // Criar plano. A ordem escolhida abre espaço: quem estava nela (ou depois) desce uma posição.
        app.MapPost("Plans", async (DBContext context, PlansRequest req, UserManager<Users> userManager, ClaimsPrincipal user) =>
        {
            var userId = userManager.GetCurrentUserId(user);
            if (userId == null) return Results.Unauthorized();

            var maxOrder = await context.Plans
                .Where(p => p.Status == true)
                .MaxAsync(p => (int?)p.Order) ?? 0;

            // Sem ordem: vai para o fim. Com ordem: fica entre 1 e fim+1, sem deixar buracos
            var order = Math.Clamp(req.Order ?? maxOrder + 1, 1, maxOrder + 1);

            var toShift = await context.Plans
                .Where(p => p.Status == true && p.Order >= order)
                .ToListAsync();

            foreach (var other in toShift)
                other.Order++;

            var plan = new Plans(req.Name, req.Description, req.Price, req.PlanTimeSpanId, req.Highlight, order);

            if (plan.Highlight == true)
                await ClearOtherHighlightsAsync(context, 0);

            context.Plans.Add(plan);
            await context.SaveChangesAsync();   // tudo numa transação só
            return Results.Ok(plan);
        });

        // Editar plano. Se vier uma ordem nova, só os planos entre a posição antiga e a nova se movem.
        app.MapPut("Plans/{id:int}", async (DBContext context, int id, PlansRequest req, UserManager<Users> userManager, ClaimsPrincipal user) =>
        {
            var userId = userManager.GetCurrentUserId(user);
            if (userId == null) return Results.Unauthorized();

            var plan = await context.Plans.FirstOrDefaultAsync(o => o.Id == id);
            if (plan == null) return Results.NotFound();

            // Sem ordem no pedido: o plano continua onde está
            if (req.Order != null && plan.Status == true)
            {
                var maxOrder = await context.Plans
                    .Where(p => p.Status == true)
                    .MaxAsync(p => (int?)p.Order) ?? 0;

                var oldOrder = plan.Order;
                var newOrder = Math.Clamp(req.Order.Value, 1, maxOrder);

                if (newOrder < oldOrder)
                {
                    // Subiu na lista: quem estava no caminho desce uma posição
                    var inTheWay = await context.Plans
                        .Where(p => p.Status == true && p.Id != id && p.Order >= newOrder && p.Order < oldOrder)
                        .ToListAsync();

                    foreach (var other in inTheWay)
                        other.Order++;
                }
                else if (newOrder > oldOrder)
                {
                    // Desceu na lista: quem estava no caminho sobe uma posição
                    var inTheWay = await context.Plans
                        .Where(p => p.Status == true && p.Id != id && p.Order > oldOrder && p.Order <= newOrder)
                        .ToListAsync();

                    foreach (var other in inTheWay)
                        other.Order--;
                }

                plan.Order = newOrder;
            }

            if (req.Highlight == true)
                await ClearOtherHighlightsAsync(context, id);

            plan.Name = req.Name;
            plan.Description = req.Description;
            plan.Price = req.Price;
            plan.PlanTimeSpanId = req.PlanTimeSpanId;
            plan.Highlight = req.Highlight;

            await context.SaveChangesAsync();
            return Results.Ok(plan);
        });

        // "Apagar" plano: desativa (Status = false) e fecha o buraco na ordem.
        // Não apaga de verdade porque contratos antigos ainda apontam para ele.
        app.MapDelete("Plans/{id:int}", async (DBContext context, int id, UserManager<Users> userManager, ClaimsPrincipal user) =>
        {
            var userId = userManager.GetCurrentUserId(user);
            if (userId == null) return Results.Unauthorized();

            var plan = await context.Plans.FirstOrDefaultAsync(o => o.Id == id);
            if (plan == null) return Results.NotFound();

            if (plan.Status == true)
            {
                var deletedOrder = plan.Order;

                var after = await context.Plans
                    .Where(p => p.Status == true && p.Id != id && p.Order > deletedOrder)
                    .ToListAsync();

                foreach (var other in after)
                    other.Order--;
            }

            plan.Status = false;
            plan.Highlight = false;

            await context.SaveChangesAsync();
            return Results.Ok();
        });
    }
}