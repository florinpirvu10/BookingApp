namespace Booking.Services;

public class ConsoleService : IConsoleService
{
    private readonly IBookingService _bookingService;
    public ConsoleService(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    public void AvailabilityScenario(string input)
    {
        var parameters = input.Substring("Availability(".Length, input.Length - "Availability()".Length);
        var commandParts = parameters.Split(',');

        if (commandParts.Length != 3)
        {
            Console.WriteLine("Invalid command. Format: Availability(<HotelID>, <Date|DateRange>, <RoomType>)");
            return;
        }

        var hotelId = commandParts[0].Trim();
        var dateRange = commandParts[1].Trim();
        var roomType = commandParts[2].Trim();

        DateTime startDate, endDate;

        if (dateRange.Contains('-') && dateRange.Split('-').Length == 2)
        {
            var dates = dateRange.Split('-');
            if (dates.Length != 2 ||
                !DateTime.TryParseExact(dates[0], "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out startDate) ||
                !DateTime.TryParseExact(dates[1], "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out endDate))
            {
                Console.WriteLine("Invalid date range format. Use YYYYMMDD-YYYYMMDD.");
                return;
            }
        }
        else
        {
            if (!DateTime.TryParseExact(dateRange, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out startDate))
            {
                Console.WriteLine("Invalid date format. Use YYYYMMDD.");
                return;
            }
            endDate = startDate;
        }

        var response = _bookingService.CheckRoomAvailability(hotelId, roomType, startDate, endDate);

        Console.WriteLine(response.IsSuccess ? response.Data : response.ErrorMessage);
    }


    public void RoomTypesScenario(string input)
    {
        var parameters = input.Substring("RoomTypes(".Length, input.Length - "RoomTypes()".Length);
        var commandParts = parameters.Split(',');

        if (commandParts.Length != 3)
        {
            Console.WriteLine("Invalid command. Format: RoomTypes(<HotelID>, <Date|DateRange>, <NumberOfPeople>)");
            return;
        }

        var hotelId = commandParts[0].Trim();
        var dateRange = commandParts[1].Trim();
        if (!int.TryParse(commandParts[2].Trim(), out int numberOfPeople) || numberOfPeople <= 0)
        {
            Console.WriteLine("Invalid number of people. It must be a positive integer.");
            return;
        }

        DateTime startDate, endDate;

        if (dateRange.Contains('-'))
        {
            var dates = dateRange.Split('-');
            if (dates.Length != 2 ||
                !DateTime.TryParseExact(dates[0], "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out startDate) ||
                !DateTime.TryParseExact(dates[1], "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out endDate))
            {
                Console.WriteLine("Invalid date range format. Use YYYYMMDD-YYYYMMDD.");
                return;
            }
        }
        else
        {
            if (!DateTime.TryParseExact(dateRange, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out startDate))
            {
                Console.WriteLine("Invalid date format. Use YYYYMMDD.");
                return;
            }
            endDate = startDate;
        }

        var response = _bookingService.GetRoomTypesForPeople(hotelId, startDate, endDate, numberOfPeople);

        Console.WriteLine(response.IsSuccess ? response.Data : response.ErrorMessage);
    }
}
