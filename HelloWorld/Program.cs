namespace HelloWorld;

// Main program entry point
public class Program
{
    static void Main()
    {
        var ageCalculator = new AgeCalculator();
        var userInterface = new ConsoleUserInterface();
        
        userInterface.WriteLine("What is your name?");
        var name = userInterface.ReadLine();
        var currentDate = DateTime.Now;
        userInterface.WriteLine($"{Environment.NewLine}Hello, {name}, on {currentDate:d} at {currentDate:t}!");

        // Process birth year twice
        ProcessBirthYear(userInterface, ageCalculator);
        ProcessBirthYear(userInterface, ageCalculator);

        userInterface.Write($"{Environment.NewLine}Press any key to exit...");
        Console.ReadKey(true);
    }

    // Refactored method to handle birth year logic - now public and testable
    public static void ProcessBirthYear(IUserInterface userInterface, IAgeCalculator ageCalculator)
    {
        userInterface.WriteLine("What is your birth year?");
        string input = userInterface.ReadLine();
        if (int.TryParse(input, out int year))
        {
            var age = ageCalculator.CalculateAge(year);
            userInterface.WriteLine($"You are approximately {age} years old.");

            if (age < 18)
            {
                userInterface.WriteLine("You are under 18 years old.");
                userInterface.WriteLine("Some content may be restricted.");
                userInterface.WriteLine("Please have a parent or guardian present.");
                for (int i = 0; i < 5; i++)
                {
                    userInterface.WriteLine($"Count: {i}");
                }
            }
            else
            {
                userInterface.WriteLine("You are 18 or older.");
                userInterface.WriteLine("All content is available to you.");
                userInterface.WriteLine("Thank you for confirming your age.");
                for (int i = 0; i < 5; i++)
                {
                    userInterface.WriteLine($"Count: {i}");
                }
            }
        }
        else
        {
            userInterface.WriteLine("Invalid input for birth year.");
        }
    }
}

// Interface for user interaction - allows for testing with mock implementations
public interface IUserInterface
{
    string ReadLine();
    void WriteLine(string message);
    void Write(string message);
}

// Concrete implementation of IUserInterface that uses the console
public class ConsoleUserInterface : IUserInterface
{
    public string ReadLine() => Console.ReadLine() ?? string.Empty;
    public void WriteLine(string message) => Console.WriteLine(message);
    public void Write(string message) => Console.Write(message);
}

// Interface for age calculation - allows for testing with mock implementations
public interface IAgeCalculator
{
    int CalculateAge(int birthYear);
}

// Concrete implementation of IAgeCalculator
public class AgeCalculator : IAgeCalculator
{
    public int CalculateAge(int birthYear) => DateTime.Now.Year - birthYear;
}