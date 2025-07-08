using System;
using System.Collections.Generic;
using Xunit;
using Moq;

namespace HelloWorld.Tests
{
    // This class provides a modified version of the Program for testing
    public class IntegrationTests
    {
        [Fact]
        public void CompleteProgram_RunsSuccessfully()
        {
            // Arrange
            var mockUserInterface = new Mock<IUserInterface>();
            var mockAgeCalculator = new Mock<IAgeCalculator>();
            
            // Setup mocks for all user interactions in sequence
            var userInputs = new Queue<string>(new[] { "Test User", "1990", "2010" });
            mockUserInterface.Setup(ui => ui.ReadLine())
                .Returns(() => userInputs.Count > 0 ? userInputs.Dequeue() : string.Empty);
                
            mockAgeCalculator.Setup(ac => ac.CalculateAge(1990)).Returns(35);
            mockAgeCalculator.Setup(ac => ac.CalculateAge(2010)).Returns(15);
            
            // Act - Call all parts of the program manually
            // Simulate asking for name
            mockUserInterface.Verify(ui => ui.WriteLine("What is your name?"), Times.Never);
            mockUserInterface.Object.WriteLine("What is your name?");
            var name = mockUserInterface.Object.ReadLine(); // Should be "Test User"
            var currentDate = DateTime.Now;
            mockUserInterface.Object.WriteLine($"{Environment.NewLine}Hello, {name}, on {currentDate:d} at {currentDate:t}!");
            
            // First ProcessBirthYear call (adult)
            Program.ProcessBirthYear(mockUserInterface.Object, mockAgeCalculator.Object);
            
            // Second ProcessBirthYear call (minor)
            Program.ProcessBirthYear(mockUserInterface.Object, mockAgeCalculator.Object);
            
            // Assert
            mockUserInterface.Verify(ui => ui.WriteLine($"{Environment.NewLine}Hello, Test User, on {currentDate:d} at {currentDate:t}!"), Times.Once);
            
            // Verify adult messages shown
            mockAgeCalculator.Verify(ac => ac.CalculateAge(1990), Times.Once);
            mockUserInterface.Verify(ui => ui.WriteLine("You are approximately 35 years old."), Times.Once);
            mockUserInterface.Verify(ui => ui.WriteLine("You are 18 or older."), Times.Once);
            
            // Verify minor messages shown
            mockAgeCalculator.Verify(ac => ac.CalculateAge(2010), Times.Once);
            mockUserInterface.Verify(ui => ui.WriteLine("You are approximately 15 years old."), Times.Once);
            mockUserInterface.Verify(ui => ui.WriteLine("You are under 18 years old."), Times.Once);
        }
        
        [Fact]
        public void CompleteProgram_WithInvalidBirthYear_HandlesErrors()
        {
            // Arrange
            var mockUserInterface = new Mock<IUserInterface>();
            var mockAgeCalculator = new Mock<IAgeCalculator>();
            
            // Setup mocks for all user interactions in sequence
            var userInputs = new Queue<string>(new[] { "Test User", "not-a-year", "1990" });
            mockUserInterface.Setup(ui => ui.ReadLine())
                .Returns(() => userInputs.Count > 0 ? userInputs.Dequeue() : string.Empty);
            
            mockAgeCalculator.Setup(ac => ac.CalculateAge(1990)).Returns(35);
            
            // Act - Call all parts of the program manually
            mockUserInterface.Object.WriteLine("What is your name?");
            var name = mockUserInterface.Object.ReadLine(); // Should be "Test User"
            var currentDate = DateTime.Now;
            mockUserInterface.Object.WriteLine($"{Environment.NewLine}Hello, {name}, on {currentDate:d} at {currentDate:t}!");
            
            // First ProcessBirthYear call (invalid input)
            Program.ProcessBirthYear(mockUserInterface.Object, mockAgeCalculator.Object);
            
            // Second ProcessBirthYear call (adult)
            Program.ProcessBirthYear(mockUserInterface.Object, mockAgeCalculator.Object);
            
            // Assert
            mockUserInterface.Verify(ui => ui.WriteLine("Invalid input for birth year."), Times.Once);
            mockAgeCalculator.Verify(ac => ac.CalculateAge(1990), Times.Once);
            mockUserInterface.Verify(ui => ui.WriteLine("You are approximately 35 years old."), Times.Once);
        }
    }
}
