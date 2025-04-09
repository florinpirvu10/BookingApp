using Booking.Entities;

namespace Booking.Services
{
    public interface IBookingService
    {
        Response<string> CheckRoomAvailability(string hotelId, string roomType, DateTime start, DateTime end);
        Response<string> GetRoomTypesForPeople(string hotelId, DateTime start, DateTime end, int numberOfPeople);
    }
}
