using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class TodoList
{
    private const string FilePath = "todo.json";
    private List<TodoItem> items;

    public TodoList()
    {
        items = LoadFromFile();
    }

    public void Add(string description)
    {
        items.Add(new TodoItem { Description = description });
        SaveToFile();
    }

    public void List()
    {
        if (items.Count == 0)
        {
            Console.WriteLine("No tasks yet.");
            return;
        }

        for (int i = 0; i < items.Count; i++)
        {
            string status = items[i].Done ? "[X]" : "[ ]";
            Console.WriteLine($"{i + 1}. {status} {items[i].Description}");
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
            if (items[i].Description.ToLower().Contains(keyword.ToLower()))
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
