using System;
using System.Collections.Generic;

public class Program{
    
    //the list of tasks we have saved
    public List<string> taskList = new List<string>();

    //the list of actions we can perform. Modify here to facilitate future additions
    public List<string> modificationsPermitted = new List<string>(){"add" , "tick"};
    
    public static void Main(string[] args){
        
        var program = new Program();

        //prevent a null or wrong input
        while(true){
            //this will need to slightly changed if we want to add future options.
            Console.WriteLine("Do you wish to add new tasks or check the as complete? Please  type 'add' or 'tick'.");
            var input = Console.ReadLine()?.Trim().toLower();

            if(input == null ||  string.IsNullOrEmpty(input)){
                Console.WriteLine("No input detected. Please Try again");
                continue;
            }

            if(modificationsPermitted.Contains(input)){
                break;
            }
            else{
                Console.WriteLine("Error: Wrote input. Please try again");
            }

        }

        //this will also need to be changed.
        switch(input){
            case "add":
            if(taskList.Count == 0){}


        }



        //intialise the first three tasks by asking the user what they are
        for(int i = 0; i < 3; i++){
            Console.WriteLine("Task Number " + (i+1) + " : What do you need to do");
            var input = Console.ReadLine();
            var task = new TodoItem(input, false);
            program.AddTask(input);
            Console.WriteLine("Task Number " + (i+1) + " added. Add " + (3 - (i+1)) + " more");       
        }      
        program.DisplayList();
    }


    public void AddTask(string taskName){
        taskList.Add(taskName);
    }
    public void DisplayList(){
        for(int i = 0; i <= taskList.Count-1; i++){
            Console.WriteLine("[] {0}",taskList[i]);
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
