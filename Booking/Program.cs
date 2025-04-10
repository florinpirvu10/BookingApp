using Booking;
using Booking.Helpers;
using Booking.Repositories;
using Booking.Services;

internal class Program
{
    private static void Main()
    {
        var fileReader = new FileReader();
        var hotelRepository = new HotelRepository(fileReader);
        var bookingRepository = new BookingRepository(fileReader);
        IBookingService bookingService = new BookingService(bookingRepository, hotelRepository);
        var consoleService = new ConsoleService(bookingService);

        var commandProcessor = new CommandProcessor(consoleService);
        commandProcessor.Run();
    }
}