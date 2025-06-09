using System;

public class TempConv{
    public float TempCelcius {get; set;}
    public float TempFahrenheit {get; set;}
  
    public TempConv(float tempCel){        
        var tempFahr =  (tempCel * 9/5) + 32;
        TempFahrenheit = tempFahr;
    }
    public void SaveCelcius(float cel){
        TempCelcius = cel;
    }

    public void DisplayTemperatures(){
        Console.WriteLine("It is {0} degrees Celcius", TempCelcius);
        Console.WriteLine("It is {0} degrees Fahrenheit", TempFahrenheit);
        
    }
}
