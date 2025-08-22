using System.Collections.Generic;
using ezra.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace ezra.Server.Data;

public class TodoDbContext : DbContext
{
    public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options) { }

    public DbSet<TodoItem> Todos => Set<TodoItem>();
}
