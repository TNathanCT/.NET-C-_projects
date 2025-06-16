using System;
using System.Collections.Generic;

public class Program{
    
    //the list of tasks we have saved
    public List<TodoItem> taskList = new List<TodoItem>();

    //the list of actions we can perform. Modify here to facilitate future additions
    public List<string> modificationsPermitted = new List<string>(){"add" , "tick", "list", "rename"};
    
    public static void Main(string[] args){
        
        var program = new Program();
        //this will need to slightly changed if we want to add future options.
        Console.WriteLine("Do you wish to add new tasks or check the as complete? Please  type 'add', 'list', 'rename' or 'tick'.");
        var input = Console.ReadLine()?.Trim().ToLower();

        //prevent a null or wrong input
        while(true){
            if(input == null || string.IsNullOrEmpty(input)){
                Console.WriteLine("No input detected. Please Try again");
                continue;
            }

            if(program.modificationsPermitted.Contains(input)){
                break;
            }
            else{
                Console.WriteLine("Error: Wrote input. Please try again");
            }

        }










        //this will also need to be changed.
        switch(input){
            case "add":
                program.AddTask();
                break;
            case "list":
                program.DisplayList();
                break;

            case "tick":
                program.CompleteTask();
                program.DisplayList();
                break;

            default:
                break;

        }   
    }


    public void AddTask(string defaultValue = "[Unnamed Task]"){
        //ensure that the first three tasks are set.
        if(taskList.Count == 0){
            AddFirstThree();
            return;//bypass the rest of the function
        }

        Console.WriteLine("What should task number " + taskList.Count + " be ?");
        var input = Console.ReadLine();
        input = string.IsNullOrWhiteSpace(input) ? defaultValue : input.Trim();

       //if(input != defaultValue){
            taskList.Add(new TodoItem (input, false));
            Console.WriteLine("Task Number " + taskList.Count + " added : " + input);
        //}   
    }

    public void AddFirstThree(string defaultValue = "[Unnamed Task]"){
        for(int i = 0; i < 3; i++){
            Console.WriteLine("Task Number " + (i+1) + " : What do you need to do");
            var input = Console.ReadLine();
            input = string.IsNullOrWhiteSpace(input) ? defaultValue : input.Trim();
            taskList.Add(new TodoItem (input, false));
            Console.WriteLine("Task Number " + (i+1) + " added. Add " + (3 - (i+1)) + " more");       
        }     
    }
    


    public void CompleteTask(){
        Console.WriteLine("Which task is complete? Please provide the number.");
        var input = Console.ReadLine();
        while (!double.TryParse(Console.ReadLine(), out number))
        {
            Console.Write("This is not valid input. Please enter an integer value: ");
        }

        taskList[input].isDone = true;        
    }




    public void DisplayList(){
        for(int i = 0; i <= taskList.Count-1; i++){
            Console.WriteLine("Task number " + (i+1) + (taskList[i].isDone ? "[X] " : "[ ] ") + taskList[i].Name);
        }
    }
}

public class TodoItem{
    public string TaskName { get; set;}
    public bool IsDone{ get; set;}

    public TodoItem(string name, bool done){
        TaskName = name;
        IsDone = done;
    }
}
