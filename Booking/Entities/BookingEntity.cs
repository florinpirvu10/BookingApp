using System.Text.Json.Serialization;

namespace Booking.Entities;

public class BookingEntity
{
    [JsonPropertyName("hotelId")]
    public required string HotelId { get; set; }

    [JsonPropertyName("arrival")]
    public required string Arrival { get; set; }

    [JsonPropertyName("departure")]
    public required string Departure { get; set; }

    [JsonPropertyName("roomType")]
    public required string RoomType { get; set; }

    [JsonPropertyName("roomRate")]
    public string RoomRate { get; set; } = String.Empty;
}
