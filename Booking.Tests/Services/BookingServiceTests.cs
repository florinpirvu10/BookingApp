using Booking.Entities;
using Booking.Repositories;
using Booking.Services;
using Moq;

namespace Booking.Tests.Services;

public class BookingServiceTests
{
    private readonly Mock<IBookingRepository> _bookingRepositoryMock;
    private readonly Mock<IHotelRepository> _hotelRepositoryMock;
    private readonly BookingService _bookingService;

    public BookingServiceTests()
    {
        _bookingRepositoryMock = new Mock<IBookingRepository>();
        _hotelRepositoryMock = new Mock<IHotelRepository>();
        _bookingService = new BookingService(_bookingRepositoryMock.Object, _hotelRepositoryMock.Object);
    }

    [Fact]
    public void CheckRoomAvailability_ShouldReturnAvailableRooms_WhenRoomsAreAvailable()
    {
        // Arrange
        var bookings = new List<BookingEntity>
        {
            new BookingEntity { HotelId = "H1", RoomType = "DBL", Arrival = "20240901", Departure = "20240903" }
        };
        var rooms = new List<Room>
        {
            new Room { RoomType = "DBL", RoomId = "201" },
            new Room { RoomType = "DBL", RoomId = "202" }
        };

        _bookingRepositoryMock.Setup(repo => repo.GetBookings()).Returns(new Response<IEnumerable<BookingEntity>>
        {
            Data = bookings,
            IsSuccess = true
        });

        _hotelRepositoryMock.Setup(repo => repo.GetHotelRooms("H1", "DBL")).Returns(new Response<IEnumerable<Room>>
        {
            Data = rooms,
            IsSuccess = true
        });

        // Act
        var result = _bookingService.CheckRoomAvailability("H1", "DBL", new DateTime(2024, 9, 1), new DateTime(2024, 9, 3));

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Available rooms: 1", result.Data);
    }

    [Fact]
    public void CheckRoomAvailability_ShouldReturnOverbooked_WhenBookingsExceedRooms()
    {
        // Arrange
        var bookings = new List<BookingEntity>
        {
            new BookingEntity { HotelId = "H1", RoomType = "DBL", Arrival = "20240901", Departure = "20240903" },
            new BookingEntity { HotelId = "H1", RoomType = "DBL", Arrival = "20240901", Departure = "20240903" }
        };
        var rooms = new List<Room>
        {
            new Room { RoomType = "DBL", RoomId = "201" }
        };

        _bookingRepositoryMock.Setup(repo => repo.GetBookings()).Returns(new Response<IEnumerable<BookingEntity>>
        {
            Data = bookings,
            IsSuccess = true
        });

        _hotelRepositoryMock.Setup(repo => repo.GetHotelRooms("H1", "DBL")).Returns(new Response<IEnumerable<Room>>
        {
            Data = rooms,
            IsSuccess = true
        });

        // Act
        var result = _bookingService.CheckRoomAvailability("H1", "DBL", new DateTime(2024, 9, 1), new DateTime(2024, 9, 3));

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Overbooked by: -1", result.Data);
    }

    [Fact]
    public void GetRoomTypesForPeople_ShouldAllocateRooms_WhenEnoughRoomsAreAvailable()
    {
        // Arrange
        var hotel = new HotelEntity
        {
            Id = "H1",
            Name = "Hotel California",
            RoomTypes = new List<RoomType>
            {
                new RoomType { Code = "DBL", Size = 2, Description = "Double room" },
                new RoomType { Code = "SGL", Size = 1, Description = "Single room" }
            },
            Rooms = new List<Room>
            {
                new Room { RoomType = "DBL", RoomId = "201" },
                new Room { RoomType = "DBL", RoomId = "202" },
                new Room { RoomType = "SGL", RoomId = "101" }
            }
        };

        var bookings = new List<BookingEntity>();

        _hotelRepositoryMock.Setup(repo => repo.GetHotelById("H1")).Returns(new Response<HotelEntity?>
        {
            Data = hotel,
            IsSuccess = true
        });

        _bookingRepositoryMock.Setup(repo => repo.GetBookings()).Returns(new Response<IEnumerable<BookingEntity>>
        {
            Data = bookings,
            IsSuccess = true
        });

        // Act
        var result = _bookingService.GetRoomTypesForPeople("H1", new DateTime(2024, 9, 1), new DateTime(2024, 9, 3), 3);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Hotel California: DBL, SGL", result.Data);
    }

    [Fact]
    public void GetRoomTypesForPeople_ShouldReturnError_WhenAllocationIsNotPossible()
    {
        // Arrange
        var hotel = new HotelEntity
        {
            Id = "H1",
            Name = "Hotel California",
            RoomTypes = new List<RoomType>
            {
                new RoomType { Code = "DBL", Size = 2, Description = "Double room" },
                new RoomType { Code = "SGL", Size = 1, Description = "Single room" }
            },
            Rooms = new List<Room>
            {
                new Room { RoomType = "DBL", RoomId = "201" }
            }
        };

        var bookings = new List<BookingEntity>();

        _hotelRepositoryMock.Setup(repo => repo.GetHotelById("H1")).Returns(new Response<HotelEntity?>
        {
            Data = hotel,
            IsSuccess = true
        });

        _bookingRepositoryMock.Setup(repo => repo.GetBookings()).Returns(new Response<IEnumerable<BookingEntity>>
        {
            Data = bookings,
            IsSuccess = true
        });

        // Act
        var result = _bookingService.GetRoomTypesForPeople("H1", new DateTime(2024, 9, 1), new DateTime(2024, 9, 3), 5);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Allocation not possible with the available rooms.", result.ErrorMessage);
    }
}
