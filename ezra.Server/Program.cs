using ezra.Server.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// EF Core (SQLite)
builder.Services.AddDbContext<TodoDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// CORS for Vite dev server (adjust port if yours differs)
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("dev", policy => policy
        .WithOrigins("https://localhost:49673", "http://localhost:49673")
        .AllowAnyHeader()
        .AllowAnyMethod());
});

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("dev");
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.MapFallbackToFile("/index.html");

// Auto-create DB / apply migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
    await db.Database.MigrateAsync();
}

app.Run();
