using Booking.Services;

public class CommandProcessor
{
    private readonly IConsoleService _consoleService;

    public CommandProcessor(IConsoleService consoleService)
    {
        _consoleService = consoleService;
    }

    public void Run()
    {
        Console.WriteLine("Enter commands to check room availability or press Enter to exit.");

        while (true)
        {
            Console.Write("> ");
            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input)) break;

            if (input.StartsWith("Availability(") && input.EndsWith(")"))
            {
                _consoleService.AvailabilityScenario(input);
                continue;
            }

            if (input.StartsWith("RoomTypes(") && input.EndsWith(")"))
            {
                _consoleService.RoomTypesScenario(input);
                continue;
            }

            Console.WriteLine("Invalid command. Use 'Availability(<HotelID>, <Date|DateRange>, <RoomType>)' or 'RoomTypes(<HotelID>, <Date|DateRange>, <NumberOfPeople>)'.");
        }
    }
}