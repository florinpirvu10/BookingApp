using System.Text.Json.Serialization;

namespace Booking.Entities
{
    public class HotelEntity
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("roomTypes")]
        public List<RoomType> RoomTypes { get; set; }

        [JsonPropertyName("rooms")]
        public List<Room> Rooms { get; set; }
    }

    public class RoomType
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("size")]
        public int Size { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("amenities")]
        public List<string> Amenities { get; set; }

        [JsonPropertyName("features")]
        public List<string> Features { get; set; }
    }

    public class Room
    {
        [JsonPropertyName("roomType")]
        public string RoomType { get; set; }

        [JsonPropertyName("roomId")]
        public string RoomId { get; set; }
    }
}