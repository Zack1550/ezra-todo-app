// TodosControllerTests.cs
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ezra.Server.Controllers;
using ezra.Server.Models;
using ezra.Server.Service;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

public class TodosControllerTests
{
    private static TodosController CreateController(out Mock<ITodoService> mock)
    {
        mock = new Mock<ITodoService>(MockBehavior.Strict);
        return new TodosController(mock.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOk_WithItems()
    {
        // Arrange
        var controller = CreateController(out var mock);
        var items = new List<TodoItem>
        {
            new() { Id = 1, Title = "A" },
            new() { Id = 2, Title = "B", IsCompleted = true }
        };

        mock.Setup(s => s.GetAllAsync()).ReturnsAsync(items);

        // Act
        var result = await controller.GetAll();

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsAssignableFrom<IEnumerable<TodoItem>>(ok.Value);
        Assert.Equal(2, payload.Count());
        mock.VerifyAll();
    }

    [Fact]
    public async Task Get_ReturnsItem_WhenFound()
    {
        var controller = CreateController(out var mock);
        var item = new TodoItem { Id = 42, Title = "Hello" };

        mock.Setup(s => s.GetByIdAsync(42)).ReturnsAsync(item);

        var result = await controller.Get(42);

        var returned = Assert.IsType<TodoItem>(result.Value);
        Assert.Equal(42, returned.Id);
        Assert.Equal("Hello", returned.Title);
        mock.VerifyAll();
    }

    [Fact]
    public async Task Get_ReturnsNotFound_WhenMissing()
    {
        var controller = CreateController(out var mock);
        mock.Setup(s => s.GetByIdAsync(7)).ReturnsAsync((TodoItem?)null);

        var result = await controller.Get(7);

        Assert.IsType<NotFoundResult>(result.Result);
        mock.VerifyAll();
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenTitleMissing()
    {
        var controller = CreateController(out var mock);

        var res1 = await controller.Create(new TodosController.CreateTodoRequest(""));
        Assert.IsType<BadRequestObjectResult>(res1.Result);

        var res2 = await controller.Create(new TodosController.CreateTodoRequest("   "));
        Assert.IsType<BadRequestObjectResult>(res2.Result);

        mock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction_WithNewItem()
    {
        var controller = CreateController(out var mock);
        var created = new TodoItem { Id = 5, Title = "New one" };

        mock.Setup(s => s.CreateAsync("New one")).ReturnsAsync(created);

        var result = await controller.Create(new TodosController.CreateTodoRequest("New one"));

        var crt = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(TodosController.Get), crt.ActionName);
        Assert.Equal(5, crt.RouteValues?["id"]);
        var payload = Assert.IsType<TodoItem>(crt.Value);
        Assert.Equal("New one", payload.Title);

        mock.VerifyAll();
    }

    [Fact]
    public async Task Update_ReturnsNoContent_WhenServiceUpdates()
    {
        var controller = CreateController(out var mock);
        mock.Setup(s => s.UpdateAsync(10, "Updated", true)).ReturnsAsync(true);

        var result = await controller.Update(
            10, new TodosController.UpdateTodoRequest("Updated", true));

        Assert.IsType<NoContentResult>(result);
        mock.VerifyAll();
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenServiceSaysMissing()
    {
        var controller = CreateController(out var mock);
        mock.Setup(s => s.UpdateAsync(99, null, null)).ReturnsAsync(false);

        var result = await controller.Update(99, new TodosController.UpdateTodoRequest(null, null));

        Assert.IsType<NotFoundResult>(result);
        mock.VerifyAll();
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenDeleted()
    {
        var controller = CreateController(out var mock);
        mock.Setup(s => s.DeleteAsync(3)).ReturnsAsync(true);

        var result = await controller.Delete(3);

        Assert.IsType<NoContentResult>(result);
        mock.VerifyAll();
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenMissing()
    {
        var controller = CreateController(out var mock);
        mock.Setup(s => s.DeleteAsync(404)).ReturnsAsync(false);

        var result = await controller.Delete(404);

        Assert.IsType<NotFoundResult>(result);
        mock.VerifyAll();
    }
}
