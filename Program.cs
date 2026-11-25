using Microsoft.AspNetCore.Identity;
using MyCourse.Models;
using MyCourse.Data;
using MyCourse.Routes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();

// Enums (cores das notas, matérias...) viajam como texto no JSON: "Blue" em vez de 7
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Usado para baixar as imagens das questões ao gerar o PDF
builder.Services.AddHttpClient();

builder.Services.AddDbContext<DBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentityApiEndpoints<Users>()
    .AddRoles<IdentityRole<Guid>>()
    .AddEntityFrameworkStores<DBContext>();
builder.Services.AddAuthorization();

var app = builder.Build();

// Cria os papéis Admin e Staff e promove a conta de Seed:AdminEmail
await RoleSeeder.SeedAsync(app.Services, app.Configuration);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapIdentityApi<Users>();
app.MapPost("/Logout", async(SignInManager<Users> signInManager, [FromBody] object empty) =>
{
    await signInManager.SignOutAsync();
    return Results.Ok();
});

// Rotas da API
app.VideosRoutes();
app.CommentsRoutes();
app.CommentAnswersRoutes();
app.NotesRoutes();
app.SettingsRoutes();
app.PlansRoutes();
app.SupportsRoutes();
app.SupportAnswersRoutes();
app.QuestionsRoutes();
app.ListsRoutes();

app.UseHttpsRedirection();

//roles
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

    foreach (var role in new[] {"Adm", "Staff", "Support", "Student"})
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole<Guid>(role));
    }
}

app.Run();
