namespace HelloWorld;

class Program
{
    static void Main()
    {
        Console.WriteLine("What is your name?");
        var name = Console.ReadLine();
        var currentDate = DateTime.Now;
        Console.WriteLine($"{Environment.NewLine}Hello, {name}, on {currentDate:d} at {currentDate:t}!");

        // Refactored duplicated block into a method
        ProcessBirthYear();

        ProcessBirthYear();

        Console.Write($"{Environment.NewLine}Press any key to exit...");
        Console.ReadKey(true);
    }

    // Refactored method to handle birth year logic
    static void ProcessBirthYear()
    {
        Console.WriteLine("What is your birth year?");
        string input = Console.ReadLine();
        if (int.TryParse(input, out int year))
        {
            var age = DateTime.Now.Year - year;
            Console.WriteLine($"You are approximately {age} years old.");

            if (age < 18)
            {
                Console.WriteLine("You are under 18 years old.");
                Console.WriteLine("Some content may be restricted.");
                Console.WriteLine("Please have a parent or guardian present.");
                for (int i = 0; i < 5; i++)
                {
                    Console.WriteLine($"Count: {i}");
                }
            }
            else
            {
                Console.WriteLine("You are 18 or older.");
                Console.WriteLine("All content is available to you.");
                Console.WriteLine("Thank you for confirming your age.");
                for (int i = 0; i < 5; i++)
                {
                    Console.WriteLine($"Count: {i}");
                }
            }
        }
        else
        {
            Console.WriteLine("Invalid input for birth year.");
        }
    }
}