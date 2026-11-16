using Microsoft.AspNetCore.Identity;
using MyCourse.Models;
using MyCourse.Data;
using MyCourse.Routes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

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

builder.Services.AddDbContext<DBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentityApiEndpoints<Users>()
    .AddRoles<IdentityRole<Guid>>()
    .AddEntityFrameworkStores<DBContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
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

app.UseHttpsRedirection();

app.Run();