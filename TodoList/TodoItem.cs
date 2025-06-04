using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

public class TodoItem
{
    //this ensures that the string cannot be null
    public string description { get; set; }
    public bool done { get; set; }
    public DateTime? dueDates { get; set; } 
    //This is optional
    public Priority priority { get; set; } = Priority.Medium;
    public List<string> tags { get; set; } = new();


    public TodoItem() { }
    public TodoItem(string descriptions, DateTime? dueDate = null){
        description = descriptions;
        done = false;
        dueDates = dueDate;
    }


    public override string ToString()
    {
        string status = done ? "[X]" : "[ ]";
        string due = dueDates.HasValue ? $" (Due: {dueDates.Value.ToShortDateString()})" : "";
        return $"{status} {description}{due}";
    }


    
}

public enum Priority{
    Low,
    Medium,
    High
}
