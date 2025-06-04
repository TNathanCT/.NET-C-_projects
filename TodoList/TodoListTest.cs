using Xunit;
using TodoAppCLI;
using System.IO;
using System.Collections.Generic;

public class TodoListTests : IDisposable
{
    private readonly List<string> testFiles = new()
    {
        "test-add.json",
        "test-remove.json",
        "test-tags.json"
    };

    [Fact]
    public void Add_AddsNewTask_TaskAppearsInList()
    {
        var todoList = new TodoList("test-add.json", reset: true);
        todoList.Add("Buy milk");

        var output = todoList.GetItems();
        Assert.Single(output);
        Assert.Equal("Buy milk", output[0].description);
    }

    [Fact]
    public void Remove_RemovesCorrectTask_TaskIsGone()
    {
        var todoList = new TodoList("test-remove.json", reset: true);
        todoList.Add("Buy Milk");
        todoList.Add("Walk Dog");

        todoList.Remove(1);

        var output = todoList.GetItems();
        Assert.Single(output);
        Assert.Equal("Walk Dog", output[0].description);
    }

    [Fact]
    public void Add_TaskWithTags_TagsAreSavedCorrectly()
    {
        var todoList = new TodoList("test-tags.json", reset: true);
        var tags = new List<string> { "work", "urgent" };

        todoList.Add("Finish report", taggys: tags);

        var output = todoList.GetItems();
        Assert.Single(output);
        Assert.Equal("Finish report", output[0].description);
        Assert.Equal(2, output[0].tags.Count);
        Assert.Contains("work", output[0].tags);
        Assert.Contains("urgent", output[0].tags);
    }

    public void Dispose()
    {
        // Cleanup all test files
        foreach (var file in testFiles)
        {
            if (File.Exists(file))
                File.Delete(file);
        }
    }
}
