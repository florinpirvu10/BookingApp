using Booking.Entities;
using Booking.Helpers;
using System.Text.Json;

namespace Booking.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly string _filePath;
        private readonly IFileReader _fileReader;

        public BookingRepository(IFileReader fileReader)
        {
            _fileReader = fileReader;
            _filePath = Path.Combine(AppContext.BaseDirectory, "Data", "bookings.json"); ;
        }

        public Response<IEnumerable<BookingEntity>> GetBookings()
        {
            try
            {
                var response = new Response<IEnumerable<BookingEntity>>();
                var jsonContent = _fileReader.ReadAllText(_filePath);
                var bookings = JsonSerializer.Deserialize<List<BookingEntity>>(jsonContent);
                response.Data = bookings ?? [];
                response.IsSuccess = true;
                return response;
            }
            catch (Exception ex)
            {
                return new Response<IEnumerable<BookingEntity>>
                {
                    IsSuccess = false,
                    ErrorMessage = $"Error reading or deserializing the file: {ex.Message}"
                };
            }
        }

        public Response<BookingEntity?> GetBookingByType(string roomType)
        {
            var bookings = GetBookings();

            return new Response<BookingEntity?>
            {
                Data = bookings.Data.FirstOrDefault(b => b.RoomType == roomType),
                IsSuccess = bookings.IsSuccess,
                ErrorMessage = bookings.ErrorMessage
            };
        }
    }
}
