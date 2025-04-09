using Booking.Entities;

namespace Booking.Repositories
{
    public interface IBookingRepository
    {
        Response<IEnumerable<BookingEntity>> GetBookings();
    }
}
