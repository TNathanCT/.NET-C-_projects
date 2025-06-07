using System;

class Program{
    static void Main(string[] args){
        Console.WriteLine("/Write your email here: ");
        var inputEmail = Console.ReadLine();
        if(String.IsNullOrEmpty(inputEmail)){
            
        }
        
        Console.WriteLine("/Write your name here: ");
        var inputName = Console.ReadLine();


        var newUser = new UserAccount(inputName, inputEmail);
        Console.WriteLine("Is Email Valid? " + newUser.IsEmailValid());
        Console.WriteLine("Created at: " + newUser.createdAt);

    }
}
