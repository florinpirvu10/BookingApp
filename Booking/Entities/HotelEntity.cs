using System.Text.Json.Serialization;

namespace Booking.Entities;

public class HotelEntity
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("roomTypes")]
    public required List<RoomType> RoomTypes { get; set; }

    [JsonPropertyName("rooms")]
    public required List<Room> Rooms { get; set; }
}

public class RoomType
{
    [JsonPropertyName("code")]
    public required string Code { get; set; }

    [JsonPropertyName("size")]
    public int Size { get; set; }

    [JsonPropertyName("description")]
    public required string Description { get; set; }

    [JsonPropertyName("amenities")]
    public List<string> Amenities { get; set; } = [];

    [JsonPropertyName("features")]
    public List<string> Features { get; set; } = [];
}

public class Room
{
    [JsonPropertyName("roomType")]
    public required string RoomType { get; set; }

    [JsonPropertyName("roomId")]
    public required string RoomId { get; set; }
}