using System.Collections.Generic;
using System.Threading.Tasks;
using ezra.Server.Controllers;
using ezra.Server.Data;
using ezra.Server.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ezra.Server.Tests;

public class TodosControllerTests
{
    private static TodoDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<TodoDbContext>()
            .UseInMemoryDatabase($"todos-{System.Guid.NewGuid()}")
            .Options;
        return new TodoDbContext(options);
    }

    [Fact]
    public async Task Create_Then_List_Should_Return_Item()
    {
        using var db = CreateDb();
        var controller = new TodosController(db);

        // CREATE
        var created = await controller.Create(new TodosController.CreateTodo("Write tests"));
        created.Result.Should().BeOfType<CreatedAtActionResult>();
        var createdAt = (CreatedAtActionResult)created.Result!;
        createdAt.Value.Should().BeOfType<TodoItem>();
        var createdItem = (TodoItem)createdAt.Value!;
        createdItem.Title.Should().Be("Write tests");
        createdItem.IsCompleted.Should().BeFalse();

        // LIST
        var listResult = await controller.GetAll();
        var list = listResult.Value ?? (listResult.Result as IEnumerable<TodoItem>);
        list.Should().ContainSingle(t => t.Title == "Write tests" && t.IsCompleted == false);
    }

    [Fact]
    public async Task Update_Toggle_Completion_Should_Persist()
    {
        using var db = CreateDb();
        var controller = new TodosController(db);

        var created = await controller.Create(new TodosController.CreateTodo("Toggle me"));
        var createdAt = (CreatedAtActionResult)created.Result!;
        var item = (TodoItem)createdAt.Value!;
        var id = item.Id;

        var updateResult = await controller.Update(id, new TodosController.UpdateTodo(null, true));
        updateResult.Should().BeOfType<NoContentResult>();

        var get = await controller.Get(id);
        get.Value.Should().NotBeNull();
        get.Value!.IsCompleted.Should().BeTrue();
    }

    [Fact]
    public async Task Delete_Should_Remove_Item()
    {
        using var db = CreateDb();
        var controller = new TodosController(db);

        var created = await controller.Create(new TodosController.CreateTodo("Delete me"));
        var createdAt = (CreatedAtActionResult)created.Result!;
        var item = (TodoItem)createdAt.Value!;
        var id = item.Id;

        var delete = await controller.Delete(id);
        delete.Should().BeOfType<NoContentResult>();

        var after = await controller.Get(id);
        after.Result.Should().BeOfType<NotFoundResult>();
    }
}
