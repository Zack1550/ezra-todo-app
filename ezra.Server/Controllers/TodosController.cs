using ezra.Server.Models;
using ezra.Server.Service;
using Microsoft.AspNetCore.Mvc;

namespace ezra.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodosController : ControllerBase
{
    private readonly ITodoService _service;
    public TodosController(ITodoService service) => _service = service;

    public record CreateTodoRequest(string Title);
    public record UpdateTodoRequest(string? Title, bool? IsCompleted);

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoItem>>> GetAll()
        => Ok(await _service.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TodoItem>> Get(int id)
        => await _service.GetByIdAsync(id) is { } todo ? todo : NotFound();

    [HttpPost]
    public async Task<ActionResult<TodoItem>> Create([FromBody] CreateTodoRequest input)
    {
        if (string.IsNullOrWhiteSpace(input.Title)) return BadRequest("Title is required.");
        var todo = await _service.CreateAsync(input.Title);
        return CreatedAtAction(nameof(Get), new { id = todo.Id }, todo);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTodoRequest input)
        => await _service.UpdateAsync(id, input.Title, input.IsCompleted) ? NoContent() : NotFound();

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
        => await _service.DeleteAsync(id) ? NoContent() : NotFound();
}
