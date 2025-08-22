using ezra.Server.Models;

namespace ezra.Server.Service
{
    public interface ITodoService
    {
        Task<IEnumerable<TodoItem>> GetAllAsync();
        Task<TodoItem?> GetByIdAsync(int id);
        Task<TodoItem> CreateAsync(string title);
        Task<bool> UpdateAsync(int id, string? title, bool? isCompleted);
        Task<bool> DeleteAsync(int id);
    }
}
