using System;

class Program
{
    static void Main(string[] args)
    {
        var todoList = new TodoAppCLI.TodoList();

        while (true)
        {
            Console.Write("\nEnter command (add, list, remove, done, search, exit): ");
            var command = Console.ReadLine()?.Trim().ToLower();

            switch (command)
            {
                case "add":
                    Console.Write("Enter task: ");
                    var task = Console.ReadLine();
                    Console.Write("Enter due date (yyyy-mm-dd) or leave empty: ");
                    string? dateInput = Console.ReadLine();
                    DateTime? dueDate = null;


                    if(DateTime.TryParse(dateInput, out DateTime parsedDate)){
                        dueDate = parsedDate;
                    }

                    if (string.IsNullOrWhiteSpace(task)){
                        Console.WriteLine("Task cannot be empty.");
                    }
                    else{
                        todoList.Add(task, dueDate);
                        Console.WriteLine("Task Added");
                    }
                    break;

                case "list":
                    todoList.List();
                    break;

                case "remove":
                    Console.Write("Enter task number to remove: ");
                    var removeInput = Console.ReadLine();
                    if (int.TryParse(removeInput, out var removeIndex)){
                        todoList.Remove(removeIndex);
                    }
                    else{
                        Console.WriteLine("Invalid number.");
                    }
                    break;

                case "done":
                    Console.Write("Enter task number to mark done: ");
                    var doneInput = Console.ReadLine();
                    if (int.TryParse(doneInput, out var doneIndex)){
                        todoList.MarkDone(doneIndex);
                    }
                    else{
                        Console.WriteLine("Invalid number.");
                    }
                    break;

                case "search":
                    Console.Write("Enter keyword: ");
                    var keyword = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(keyword)){
                        Console.WriteLine("Please enter a valid keyword.");
                    }
                    else{
                        todoList.Search(keyword);
                    }
                    break;

                case "exit":
                    return;

                default:
                    Console.WriteLine("Unknown command.");
                    break;
            }
        }
    }
}
