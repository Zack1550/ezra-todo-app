using ezra.Server.Data;
using ezra.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ezra.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodosController : ControllerBase
{
    private readonly TodoDbContext _db;
    public TodosController(TodoDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoItem>>> GetAll()
        => await _db.Todos.OrderBy(t => t.IsCompleted).ThenByDescending(t => t.Id).ToListAsync();

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TodoItem>> Get(int id)
        => await _db.Todos.FindAsync(id) is { } todo ? todo : NotFound();

    public record CreateTodo(string Title);

    [HttpPost]
    public async Task<ActionResult<TodoItem>> Create(CreateTodo input)
    {
        if (string.IsNullOrWhiteSpace(input.Title)) return BadRequest("Title is required.");
        var todo = new TodoItem { Title = input.Title.Trim() };
        _db.Todos.Add(todo);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = todo.Id }, todo);
    }

    public record UpdateTodo(string? Title, bool? IsCompleted);

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateTodo input)
    {
        var todo = await _db.Todos.FindAsync(id);
        if (todo is null) return NotFound();

        if (input.Title is not null) todo.Title = input.Title.Trim();
        if (input.IsCompleted is not null) todo.IsCompleted = input.IsCompleted.Value;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var todo = await _db.Todos.FindAsync(id);
        if (todo is null) return NotFound();
        _db.Remove(todo);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
