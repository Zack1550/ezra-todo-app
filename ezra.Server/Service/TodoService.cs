using ezra.Server.Data;
using ezra.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace ezra.Server.Service
{
    public class TodoService : ITodoService
    {
        private readonly TodoDbContext _db;
        public TodoService(TodoDbContext db) => _db = db;

        public async Task<IEnumerable<TodoItem>> GetAllAsync()
            => await _db.Todos.OrderBy(t => t.IsCompleted).ThenByDescending(t => t.Id).ToListAsync();

        public async Task<TodoItem?> GetByIdAsync(int id)
            => await _db.Todos.FindAsync(id);

        public async Task<TodoItem> CreateAsync(string title)
        {
            var todo = new TodoItem { Title = title.Trim() };
            _db.Todos.Add(todo);
            await _db.SaveChangesAsync();
            return todo;
        }

        public async Task<bool> UpdateAsync(int id, string? title, bool? isCompleted)
        {
            var todo = await _db.Todos.FindAsync(id);
            if (todo is null) return false;
            if (title is not null) todo.Title = title.Trim();
            if (isCompleted is not null) todo.IsCompleted = isCompleted.Value;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var todo = await _db.Todos.FindAsync(id);
            if (todo is null) return false;
            _db.Remove(todo);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
