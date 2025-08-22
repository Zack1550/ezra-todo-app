using ezra.Server.Data;
using ezra.Server.Service;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class TodoServiceTests
{
    private static TodoService CreateService(string dbName)
    {
        var options = new DbContextOptionsBuilder<TodoDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
        var context = new TodoDbContext(options);
        return new TodoService(context);
    }

    [Fact]
    public async Task CreateAsync_AddsItem()
    {
        var service = CreateService(nameof(CreateAsync_AddsItem));

        var todo = await service.CreateAsync("Test item");

        Assert.Equal("Test item", todo.Title);
        Assert.False(todo.IsCompleted);

        // Verify it’s actually in the DB
        var all = await service.GetAllAsync();
        Assert.Single(all);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesFields()
    {
        var service = CreateService(nameof(UpdateAsync_UpdatesFields));
        var todo = await service.CreateAsync("Old Title");

        var result = await service.UpdateAsync(todo.Id, "New Title", true);

        Assert.True(result);
        var updated = await service.GetByIdAsync(todo.Id);
        Assert.Equal("New Title", updated.Title);
        Assert.True(updated.IsCompleted);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsFalse_WhenNotFound()
    {
        var service = CreateService(nameof(UpdateAsync_ReturnsFalse_WhenNotFound));

        var result = await service.UpdateAsync(999, "X", false);

        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_RemovesItem()
    {
        var service = CreateService(nameof(DeleteAsync_RemovesItem));
        var todo = await service.CreateAsync("To be deleted");

        var deleted = await service.DeleteAsync(todo.Id);

        Assert.True(deleted);
        Assert.Null(await service.GetByIdAsync(todo.Id));
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenMissing()
    {
        var service = CreateService(nameof(DeleteAsync_ReturnsFalse_WhenMissing));
        var deleted = await service.DeleteAsync(123);
        Assert.False(deleted);
    }
}
