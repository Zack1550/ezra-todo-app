using ezra.Server.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- Use SQLite in ezra.Server/AppData/todo.db ---
var dataDir = Path.Combine(builder.Environment.ContentRootPath, "AppData");
Directory.CreateDirectory(dataDir); // ensure folder exists
var dbPath = Path.Combine(dataDir, "todo.db");
var connectionString = $"Data Source={dbPath}";

builder.Services.AddDbContext<TodoDbContext>(opt =>
    opt.UseSqlite(connectionString));


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
