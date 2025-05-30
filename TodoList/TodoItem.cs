using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

public class TodoItem
{
    //this ensures that the string cannot be null
    public required string Description { get; set; }
    public bool Done { get; set; }

    public DateTime? dueDate { get; set; } 
    //This is optional
}
