using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace TodoAppCLI{
public class TodoList
{
    private string FilePath;
    private List<TodoItem> items;

    public TodoList(string filePath = "todo.json", bool reset = false)
    {
        this.FilePath = filePath;

        if(reset == true){
            File.WriteAllText(filePath,"[]");
            items = new List<TodoItem>();
        }
        else{
            items = LoadFromFile();
        }


        if(!reset && File.Exists(filePath)){
            string json = File.ReadAllText(filePath);

            //if below fails, it defaults to an empty list.
            items = JsonSerializer.Deserialize<List<TodoItem>>(json) ?? new List<TodoItem>();
        }
        if(reset){
            File.WriteAllText(filePath, "[]");
        }
    }

    public void Add(string description, DateTime? dueDate = null, Priority prio = Priority.Medium, List<string>? taggys = null){
        items.Add(new TodoItem { Description = description, dueDates = dueDate, priority = prio, tags = taggys ?? new List<string>() });
        SaveToFile();
    }

    public void SetDueDate(int index, DateTime dueDate){
        if(index < 1 || index > items.Count){
            Console.WriteLine("Invalid number");
            return;
        }

        items[index - 1].dueDates = dueDate;
        SaveToFile();
        Console.WriteLine($"Due date for task {index} set to {dueDate:yyyy-MM-dd}");
    }


    
    public void List()
    {
        if (items.Count == 0)
        {
            Console.WriteLine("No tasks yet.");
            return;
        }

        for (int i = 0; i < items.Count; i++){
            string status = items[i].Done ? "[X]" : "[ ]";
            string? due = items[i].dueDates.HasValue ? $" (Due: {items[i].dueDates.Value:yyyy-MM-dd})":"";
            string priority =  $" [{items[i].priority}]";
            string tagStr = items[i].tags.Count > 0 ? $" [Tags: {string.Join(", ", items[i].tags)}]" : "";
            Console.WriteLine($"{i + 1}. {status} {items[i].Description}{due}{tagStr}");
        }
    }

    public void SearchByTag(string tag){
        var found = false;
        for(int i = 0; i < items.Count; i++){
            //ignoring case of the input
            if(items[i].tags.Any(t => t.Equals(tag, StringComparison.OrdinalIgnoreCase))){
                Console.WriteLine($"{i + 1}. [{(items[i].Done ? "X" : " ")}] {items[i].Description}");
                found = true;
            }
        }

        if(!found){
            Console.WriteLine("No matching tags found.");
        }
    }


    public void Remove(int index)
    {
        if (index < 1 || index > items.Count)
        {
            Console.WriteLine("Invalid index.");
            return;
        }

        items.RemoveAt(index - 1);
        SaveToFile();
    }

    public void MarkDone(int index)
    {
        if (index < 1 || index > items.Count)
        {
            Console.WriteLine("Invalid index.");
            return;
        }

        items[index - 1].Done = true;
        SaveToFile();
    }

    public void Search(string keyword)
    {
        var found = false;
        for (int i = 0; i < items.Count; i++)
        {   
            //If the description is null, then treat it like an empty string
           if ((items[i].Description ?? "").ToLower().Contains(keyword.ToLower()))
            {
                Console.WriteLine($"{i + 1}. [{(items[i].Done ? "X" : " ")}] {items[i].Description}");
                found = true;
            }
        }

        if (!found)
            Console.WriteLine("No matching tasks found.");
    }

    private void SaveToFile()
    {
        var json = JsonSerializer.Serialize(items, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(FilePath, json);
    }

    private List<TodoItem> LoadFromFile()
    {
        if (!File.Exists(FilePath)) return new List<TodoItem>();

        var json = File.ReadAllText(FilePath);
        return JsonSerializer.Deserialize<List<TodoItem>>(json) ?? new List<TodoItem>();
    }

    public List<TodoItem> GetItems()
    {
        return items;
    }

}
}
