using Booking.Entities;

namespace Booking.Repositories
{
    public interface IHotelRepository
    {
        Response<IEnumerable<Room>> GetHotelRooms(string id, string roomType);
        Response<HotelEntity?> GetHotelById(string id);
    }
}
