using Xunit;
using TodoAppCLI;

public class UnitTest1
{
    [Fact]
    public void Add_AddsNewTask_TaskAppearsInList()
    {
        // Arrange
        var todoList = new TodoList("test-add.json", reset: true);

        // Act
        todoList.Add("Buy milk");

        // Assert
        var output = todoList.GetItems(); // You need to add this method to TodoList
        Assert.Single(output);
        Assert.Equal("Buy milk", output[0].description);
    }

    [Fact]//this means - Hey, this method is a unit test. Please run it when I call dotnet test


//This means that the function will be run twice for the test - each for the following option
/*  
[Theory]
[InlineData(2, 3, 5)]
[InlineData(10, 5, 15)]
public void AddsNumbers(int a, int b, int expected)
{
    Assert.Equal(expected, a + b);
}
*/
public void Remove_RemovesCorrectTask_TaskIsGone(){
        var todoList = new TodoList("test-remove.json", reset: true);
        todoList.Add("Buy Milk");
        todoList.Add("Walk Dog");

        todoList.Remove(1);

        var output = todoList.GetItems();
        Assert.Single(output);
        Assert.Equal("Walk Dog", output[0].description);
        //The Assert class here tells us that "Walk Dog" is expected, and the output[0].description is what is actually showing.
        //If they are not the same, return an error. 
    }



[Fact]
public void Add_TaskWithTags_TagsAreSavedCorrectly()
{
    // Arrange
    var todoList = new TodoList("test-tags.json", reset: true);
    var tags = new List<string> { "work", "urgent" };

    // Act
    todoList.Add("Finish report", taggys: tags);

    // Assert
    var items = todoList.GetItems();
    Assert.Single(items);
    Assert.Equal("Finish report", items[0].description);
    Assert.Equal(2, items[0].tags.Count);
    Assert.Contains("work", items[0].tags);
    Assert.Contains("urgent", items[0].tags);
}

}
