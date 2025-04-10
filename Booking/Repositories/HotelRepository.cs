using Booking.Entities;
using Booking.Helpers;
using System.Text.Json;

namespace Booking.Repositories;

public class HotelRepository : IHotelRepository
{
    private readonly string _filePath;
    private readonly IFileReader _fileReader;

    public HotelRepository(IFileReader fileReader)
    {
        _fileReader = fileReader;
        _filePath = Path.Combine(AppContext.BaseDirectory, "Data", "hotels.json"); ;
    }

    public Response<IEnumerable<HotelEntity>> GetHotels()
    {
        try
        {
            var response = new Response<IEnumerable<HotelEntity>>();
            var jsonContent = _fileReader.ReadAllText(_filePath);
            var hotels = JsonSerializer.Deserialize<List<HotelEntity>>(jsonContent);
            response.Data = hotels ?? [];
            response.IsSuccess = true;
            return response;
        }
        catch (Exception ex)
        {
            return new Response<IEnumerable<HotelEntity>>
            {
                IsSuccess = false,
                ErrorMessage = $"Error reading or deserializing the file: {ex.Message}"
            };
        }
    }

    public Response<IEnumerable<Room>> GetHotelRooms(string id, string? roomType)
    {
        var response = new Response<IEnumerable<Room>>();
        var hotel = GetHotelById(id);
        if (!hotel.IsSuccess)
        {
            response.ErrorMessage = hotel.ErrorMessage;
            return response;
        }

        if (hotel.Data == null)
        {
            response.ErrorMessage = $"Hotel with ID {id} not found.";
            return response;
        }

        var rooms = string.IsNullOrEmpty(roomType)
                        ? hotel.Data.Rooms
                        : hotel.Data.Rooms.Where(r => r.RoomType == roomType).ToList();

        if (rooms.Count == 0)
        {
            response.ErrorMessage = roomType == null
                ? $"No rooms found for hotel with ID {id}."
                : $"No rooms found for hotel with ID {id} and room type {roomType}.";
            return response;
        }

        response.Data = rooms;
        response.IsSuccess = true;
        return response;
    }

    public Response<HotelEntity?> GetHotelById(string id)
    {
        var hotels = GetHotels();

        return new Response<HotelEntity?>
        {
            Data = hotels.Data.FirstOrDefault(b => b.Id == id),
            IsSuccess = hotels.IsSuccess,
            ErrorMessage = hotels.ErrorMessage
        };
    }
}
