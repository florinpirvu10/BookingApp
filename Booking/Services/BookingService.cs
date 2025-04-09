using Booking.Entities;
using Booking.Repositories;

namespace Booking.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IHotelRepository _hotelRepository;

        public BookingService(IBookingRepository bookingRepository, IHotelRepository hotelRepository)
        {
            _bookingRepository = bookingRepository;
            _hotelRepository = hotelRepository;
        }

        public Response<string> CheckRoomAvailability(string hotelId, string roomType, DateTime start, DateTime end)
        {
            var response = new Response<string>();

            var bookings = _bookingRepository.GetBookings();
            if (!bookings.IsSuccess)
            {
                response.ErrorMessage = bookings.ErrorMessage;
                return response;
            }

            var rooms = _hotelRepository.GetHotelRooms(hotelId, roomType);
            if (!rooms.IsSuccess)
            {
                response.ErrorMessage = rooms.ErrorMessage;
                return response;
            }

            var numberOfBookings = CountBookings(bookings.Data, hotelId, roomType, start, end);

            var typeRooms = rooms.Data
                .Where(room => room.RoomType == roomType)
                .Count();

            int availableRooms = typeRooms - numberOfBookings;

            if (availableRooms < 0)
            {
                response.Data = $"Overbooked by: {availableRooms}";
                response.IsSuccess = true;
                return response;
            }

            if (availableRooms > 0)
            {
                response.Data = $"Available rooms: {availableRooms}";
            }
            else
            {
                response.Data = "No rooms available for the specified criteria.";
            }

            response.IsSuccess = true;
            return response;
        }

        public Response<string> GetRoomTypesForPeople(string hotelId, DateTime start, DateTime end, int numberOfPeople)
        {
            var response = new Response<string>();

            var hotel = _hotelRepository.GetHotelById(hotelId);
            if (!hotel.IsSuccess || hotel.Data == null)
            {
                response.ErrorMessage = hotel.ErrorMessage ?? $"Hotel with ID {hotelId} not found.";
                return response;
            }

            var bookings = _bookingRepository.GetBookings();
            if (!bookings.IsSuccess)
            {
                response.ErrorMessage = bookings.ErrorMessage;
                return response;
            }

            var conflictingRoomTypes = bookings.Data
                .Where(b => b.HotelId == hotelId)
                .Where(b =>
                    DateTime.TryParseExact(b.Arrival, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime arrival) &&
                    DateTime.TryParseExact(b.Departure, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime departure) &&
                    !(departure <= start || arrival >= end))
                .Select(b => b.RoomType);

            var availableRoomTypes = hotel.Data.Rooms
                .GroupBy(room => room.RoomType)
                .Select(group => new
                {
                    RoomType = group.Key,
                    Count = group.Count() - conflictingRoomTypes.Where(x => x == group.Key).Count(),
                    Capacity = hotel.Data.RoomTypes.First(rt => rt.Code == group.Key).Size
                })
                .OrderByDescending(rt => rt.Capacity)
                .ToList();

            var allocatedRooms = new List<string>();
            int remainingPeople = numberOfPeople;

            foreach (var roomType in availableRoomTypes)
            {
                int rooms = roomType.Count;
                while (remainingPeople > 0 && rooms > 0)
                {
                    if (remainingPeople >= roomType.Capacity)
                    {
                        allocatedRooms.Add(roomType.RoomType);
                        remainingPeople -= roomType.Capacity;
                        rooms--;
                    }
                    else if(availableRoomTypes.Any(x => x.Capacity == remainingPeople))
                    {
                        break;
                    }
                    else
                    {
                        allocatedRooms.Add(roomType.RoomType + "!");
                        remainingPeople = 0;
                    }
                }
            }

            if (remainingPeople > 0)
            {
                response.ErrorMessage = "Allocation not possible with the available rooms.";
                return response;
            }

            response.Data = $"{hotel.Data.Name}: {string.Join(", ", allocatedRooms)}";
            response.IsSuccess = true;
            return response;
        }

        protected int CountBookings(IEnumerable<BookingEntity> bookings, string hotelId, string roomType, DateTime start, DateTime end)
        {
            return bookings
                 .Where(b => b.HotelId == hotelId && b.RoomType == roomType)
                 .Count(b =>
                     DateTime.TryParseExact(b.Arrival, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime arrival) &&
                     DateTime.TryParseExact(b.Departure, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime departure) &&
                     !(departure <= start || arrival >= end));
        }
    }
}
