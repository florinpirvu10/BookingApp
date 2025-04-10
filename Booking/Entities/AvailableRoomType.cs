namespace Booking.Entities;

public class AvailableRoomType
{
    public required string RoomType { get; set; }
    public int Count { get; set; }
    public int Capacity { get; set; }
}
