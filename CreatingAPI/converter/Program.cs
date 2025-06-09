using System;

public class Program{
    
    static void Main(string[] args){

        Console.WriteLine("Write temperature in Celcius: ");
        var input = float.Parse(Console.ReadLine());
        var tempConv = new TempConv(input);
        
        tempConv.SaveCelcius(input);
        tempConv.DisplayTemperatures();
    }

}
