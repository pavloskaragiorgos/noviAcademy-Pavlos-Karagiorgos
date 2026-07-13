using System.Text.Json.Serialization;
using WorldRank.Application;
using WorldRank.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Reuses the same Application/Infrastructure DI wiring as the Console app
// (in-memory repositories for now — swap for DB-backed ones later without touching this file).
builder.Services.AddApplication();
builder.Services.AddInfrastructure();

builder.Services.AddMemoryCache();

// Accept/emit enums (e.g. Currency) as their string names, not numbers.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Swagger / OpenAPI — interactive API docs at /swagger.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Serve the Swagger JSON and UI in Development.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapGet("/", () => Results.Redirect("/swagger")); // root → Swagger UI
}

app.MapControllers();

app.Run();
