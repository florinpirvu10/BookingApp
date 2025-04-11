using Booking.Services;
using Moq;

namespace Booking.Tests;

public class CommandProcessorTests
{
    private readonly Mock<IConsoleService> _consoleServiceMock;
    private readonly CommandProcessor _commandProcessor;

    public CommandProcessorTests()
    {
        _consoleServiceMock = new Mock<IConsoleService>();
        _commandProcessor = new CommandProcessor(_consoleServiceMock.Object);
    }

    [Fact]
    public void Run_ShouldExit_WhenInputIsEmpty()
    {
        // Arrange
        using var consoleInput = new StringReader("\n");
        Console.SetIn(consoleInput);

        using var consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);

        // Act
        _commandProcessor.Run();

        // Assert
        Assert.Contains("Enter commands to check room availability or press Enter to exit.", consoleOutput.ToString());

    }

    [Fact]
    public void Run_ShouldCallAvailabilityScenario_WhenInputIsAvailabilityCommand()
    {

        // Arrange
        using var consoleInput = new StringReader("Availability(H1, 20240901, DBL)\n\n");
        Console.SetIn(consoleInput);


        using var consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);

        // Act
        _commandProcessor.Run();

        // Assert
        _consoleServiceMock.Verify(service => service.AvailabilityScenario("Availability(H1, 20240901, DBL)"), Times.Once);
    }

    [Fact]
    public void Run_ShouldCallRoomTypesScenario_WhenInputIsRoomTypesCommand()
    {
        // Arrange
        using var consoleInput = new StringReader("RoomTypes(H1, 20240901, 3)\n\n");
        Console.SetIn(consoleInput);

        using var consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);

        // Act
        _commandProcessor.Run();

        // Assert
        _consoleServiceMock.Verify(service => service.RoomTypesScenario("RoomTypes(H1, 20240901, 3)"), Times.Once);
    }

    [Fact]
    public void Run_ShouldPrintError_WhenInputIsInvalidCommand()
    {
        // Arrange
        using var consoleInput = new StringReader("InvalidCommand\n\n");
        Console.SetIn(consoleInput);

        using var consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);

        // Act
        _commandProcessor.Run();

        // Assert
        Assert.Contains("Invalid command. Use 'Availability(<HotelID>, <Date|DateRange>, <RoomType>)' or 'RoomTypes(<HotelID>, <Date|DateRange>, <NumberOfPeople>)'.", consoleOutput.ToString());
    }
}