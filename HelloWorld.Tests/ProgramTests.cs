using Xunit;
using Moq;
using System.IO;

namespace HelloWorld.Tests;

public class ProgramTests
{
    [Fact]
    public void ProcessBirthYear_ValidInput_CalculatesAgeCorrectly()
    {
        // Arrange
        var mockUserInterface = new Mock<IUserInterface>();
        var mockAgeCalculator = new Mock<IAgeCalculator>();
        
        // Setup mock to return "1990" when ReadLine is called
        mockUserInterface.Setup(ui => ui.ReadLine()).Returns("1990");
        
        // Setup mock to return 35 when CalculateAge is called with 1990
        mockAgeCalculator.Setup(ac => ac.CalculateAge(1990)).Returns(35);
        
        // Act
        Program.ProcessBirthYear(mockUserInterface.Object, mockAgeCalculator.Object);
        
        // Assert
        mockAgeCalculator.Verify(ac => ac.CalculateAge(1990), Times.Once);
        mockUserInterface.Verify(ui => ui.WriteLine("You are approximately 35 years old."), Times.Once);
        mockUserInterface.Verify(ui => ui.WriteLine("You are 18 or older."), Times.Once);
    }
    
    [Fact]
    public void ProcessBirthYear_InvalidInput_DisplaysErrorMessage()
    {
        // Arrange
        var mockUserInterface = new Mock<IUserInterface>();
        var mockAgeCalculator = new Mock<IAgeCalculator>();
        
        // Setup mock to return "not-a-number" when ReadLine is called
        mockUserInterface.Setup(ui => ui.ReadLine()).Returns("not-a-number");
        
        // Act
        Program.ProcessBirthYear(mockUserInterface.Object, mockAgeCalculator.Object);
        
        // Assert
        mockAgeCalculator.Verify(ac => ac.CalculateAge(It.IsAny<int>()), Times.Never);
        mockUserInterface.Verify(ui => ui.WriteLine("Invalid input for birth year."), Times.Once);
    }
    
    [Fact]
    public void ProcessBirthYear_UnderageUser_DisplaysAppropriateMessages()
    {
        // Arrange
        var mockUserInterface = new Mock<IUserInterface>();
        var mockAgeCalculator = new Mock<IAgeCalculator>();
        
        // Setup mock to return "2010" when ReadLine is called
        mockUserInterface.Setup(ui => ui.ReadLine()).Returns("2010");
        
        // Setup mock to return 15 when CalculateAge is called with 2010
        mockAgeCalculator.Setup(ac => ac.CalculateAge(2010)).Returns(15);
        
        // Act
        Program.ProcessBirthYear(mockUserInterface.Object, mockAgeCalculator.Object);
        
        // Assert
        mockAgeCalculator.Verify(ac => ac.CalculateAge(2010), Times.Once);
        mockUserInterface.Verify(ui => ui.WriteLine("You are approximately 15 years old."), Times.Once);
        mockUserInterface.Verify(ui => ui.WriteLine("You are under 18 years old."), Times.Once);
        mockUserInterface.Verify(ui => ui.WriteLine("Some content may be restricted."), Times.Once);
    }
    
    [Fact]
    public void ProcessBirthYear_VerifyCounterLoopForAdult()
    {
        // Arrange
        var mockUserInterface = new Mock<IUserInterface>();
        var mockAgeCalculator = new Mock<IAgeCalculator>();
        
        mockUserInterface.Setup(ui => ui.ReadLine()).Returns("1990");
        mockAgeCalculator.Setup(ac => ac.CalculateAge(1990)).Returns(35);
        
        // Act
        Program.ProcessBirthYear(mockUserInterface.Object, mockAgeCalculator.Object);
        
        // Assert
        // Verify that the counter messages are displayed for adults
        for (int i = 0; i < 5; i++)
        {
            mockUserInterface.Verify(ui => ui.WriteLine($"Count: {i}"), Times.Once);
        }
    }
    
    [Fact]
    public void ProcessBirthYear_VerifyCounterLoopForMinor()
    {
        // Arrange
        var mockUserInterface = new Mock<IUserInterface>();
        var mockAgeCalculator = new Mock<IAgeCalculator>();
        
        mockUserInterface.Setup(ui => ui.ReadLine()).Returns("2010");
        mockAgeCalculator.Setup(ac => ac.CalculateAge(2010)).Returns(15);
        
        // Act
        Program.ProcessBirthYear(mockUserInterface.Object, mockAgeCalculator.Object);
        
        // Assert
        // Verify that the counter messages are displayed for minors
        for (int i = 0; i < 5; i++)
        {
            mockUserInterface.Verify(ui => ui.WriteLine($"Count: {i}"), Times.Once);
        }
        mockUserInterface.Verify(ui => ui.WriteLine("Please have a parent or guardian present."), Times.Once);
    }
    
    [Fact]
    public void ProcessBirthYear_ExactlyEighteenYearsOld_DisplaysAdultMessages()
    {
        // Arrange
        var mockUserInterface = new Mock<IUserInterface>();
        var mockAgeCalculator = new Mock<IAgeCalculator>();
        
        // Setup mock to return a year that would make the person exactly 18
        mockUserInterface.Setup(ui => ui.ReadLine()).Returns("2006");
        mockAgeCalculator.Setup(ac => ac.CalculateAge(2006)).Returns(18);
        
        // Act
        Program.ProcessBirthYear(mockUserInterface.Object, mockAgeCalculator.Object);
        
        // Assert
        mockAgeCalculator.Verify(ac => ac.CalculateAge(2006), Times.Once);
        mockUserInterface.Verify(ui => ui.WriteLine("You are approximately 18 years old."), Times.Once);
        mockUserInterface.Verify(ui => ui.WriteLine("You are 18 or older."), Times.Once);
        mockUserInterface.Verify(ui => ui.WriteLine("All content is available to you."), Times.Once);
        mockUserInterface.Verify(ui => ui.WriteLine("Thank you for confirming your age."), Times.Once);
    }
}

public class AgeCalculatorTests
{
    [Fact]
    public void CalculateAge_ReturnsCorrectAge()
    {
        // Arrange
        var calculator = new AgeCalculator();
        var currentYear = DateTime.Now.Year;
        var birthYear = currentYear - 30;
        
        // Act
        var age = calculator.CalculateAge(birthYear);
        
        // Assert
        Assert.Equal(30, age);
    }
    
    [Fact]
    public void CalculateAge_WithFutureBirthYear_ReturnsNegativeAge()
    {
        // Arrange
        var calculator = new AgeCalculator();
        var currentYear = DateTime.Now.Year;
        var futureBirthYear = currentYear + 10;
        
        // Act
        var age = calculator.CalculateAge(futureBirthYear);
        
        // Assert
        Assert.Equal(-10, age);
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    public void CalculateAge_WithZeroOrNegativeBirthYear_ReturnsLargePositiveAge(int birthYear)
    {
        // Arrange
        var calculator = new AgeCalculator();
        var currentYear = DateTime.Now.Year;
        
        // Act
        var age = calculator.CalculateAge(birthYear);
        
        // Assert
        Assert.True(age >= currentYear);
        Assert.Equal(currentYear - birthYear, age);
    }
    
    [Fact]
    public void CalculateAge_WithCurrentYear_ReturnsZero()
    {
        // Arrange
        var calculator = new AgeCalculator();
        var currentYear = DateTime.Now.Year;
        
        // Act
        var age = calculator.CalculateAge(currentYear);
        
        // Assert
        Assert.Equal(0, age);
    }
}

public class ConsoleUserInterfaceTests
{
    [Fact]
    public void WriteLine_WritesToConsole()
    {
        // Arrange
        var userInterface = new ConsoleUserInterface();
        var consoleOutput = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(consoleOutput);
        
        try
        {
            // Act
            userInterface.WriteLine("Test message");
            
            // Assert
            Assert.Equal("Test message" + Environment.NewLine, consoleOutput.ToString());
        }
        finally
        {
            // Restore console output
            Console.SetOut(originalOut);
        }
    }
    
    [Fact]
    public void Write_WritesToConsole()
    {
        // Arrange
        var userInterface = new ConsoleUserInterface();
        var consoleOutput = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(consoleOutput);
        
        try
        {
            // Act
            userInterface.Write("Test message");
            
            // Assert
            Assert.Equal("Test message", consoleOutput.ToString());
        }
        finally
        {
            // Restore console output
            Console.SetOut(originalOut);
        }
    }
}
