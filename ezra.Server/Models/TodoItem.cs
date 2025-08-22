namespace ezra.Server.Models;

public class TodoItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; } = false;
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
}
