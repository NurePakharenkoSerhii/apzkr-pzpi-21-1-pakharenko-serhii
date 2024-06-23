namespace IoT.Entities;

public class Greetings
{
    public void GreetUser()
    {
        Console.WriteLine("============================================");
        Console.WriteLine("===================GUARDIC==================");
        Console.WriteLine("==========Guards management system==========");
        Console.WriteLine("============================================");
        Console.WriteLine("Greetings!");
    }

    public void AskForIdCard()
    {
        Console.WriteLine("Please use your ID card to verify your identity.");
    }
    
    public void CheckingUserExistence()
    {
        Console.WriteLine("Checking user existence");
    }

    public void LoadingLine()
    {
        for (var i = 0; i < 15; i++)
        {
            Console.Write("=");
            Task.Delay(100);
        }
        Console.WriteLine("\n");
    }

    public void GettingInventoryMessage()
    {
        Console.WriteLine("Getting your inventory");
    }

    public void ShowSuccessMessage()
    {
        Console.WriteLine("Success!");
    }
}