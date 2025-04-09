using Booking.Helpers;
using Booking.Repositories;
using Moq;

namespace Booking.Tests.Repositories;
public class BookingRepositoryTests
{
    private readonly Mock<IFileReader> _fileReaderMock;
    private readonly BookingRepository _bookingRepository;

    public BookingRepositoryTests()
    {
        _fileReaderMock = new Mock<IFileReader>();
        _bookingRepository = new BookingRepository(_fileReaderMock.Object);
    }

    [Fact]
    public void GetBookings_ShouldReturnBookings_WhenJsonIsValid()
    {
        // Arrange
        var validJson = @"
        [
            {
                ""hotelId"": ""H1"",
                ""arrival"": ""20240901"",
                ""departure"": ""20240903"",
                ""roomType"": ""DBL"",
                ""roomRate"": ""Prepaid""
            },
            {
                ""hotelId"": ""H2"",
                ""arrival"": ""20240905"",
                ""departure"": ""20240907"",
                ""roomType"": ""SGL"",
                ""roomRate"": ""Standard""
            }
        ]";
        _fileReaderMock.Setup(fr => fr.ReadAllText(It.IsAny<string>())).Returns(validJson);

        // Act
        var result = _bookingRepository.GetBookings();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Count());
        Assert.Equal("H1", result.Data.First().HotelId);
    }

    [Fact]
    public void GetBookings_ShouldReturnError_WhenJsonIsInvalid()
    {
        // Arrange
        var invalidJson = "Invalid JSON";
        _fileReaderMock.Setup(fr => fr.ReadAllText(It.IsAny<string>())).Returns(invalidJson);

        // Act
        var result = _bookingRepository.GetBookings();

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Error reading or deserializing the file", result.ErrorMessage);
    }

    [Fact]
    public void GetBookings_ShouldReturnError_WhenFileIsEmpty()
    {
        // Arrange
        var emptyJson = "";
        _fileReaderMock.Setup(fr => fr.ReadAllText(It.IsAny<string>())).Returns(emptyJson);

        // Act
        var result = _bookingRepository.GetBookings();

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Error reading or deserializing the file", result.ErrorMessage);
    }

    [Fact]
    public void GetBookingByType_ShouldReturnBooking_WhenRoomTypeExists()
    {
        // Arrange
        var validJson = @"
        [
            {
                ""hotelId"": ""H1"",
                ""arrival"": ""20240901"",
                ""departure"": ""20240903"",
                ""roomType"": ""DBL"",
                ""roomRate"": ""Prepaid""
            },
            {
                ""hotelId"": ""H2"",
                ""arrival"": ""20240905"",
                ""departure"": ""20240907"",
                ""roomType"": ""SGL"",
                ""roomRate"": ""Standard""
            }
        ]";
        _fileReaderMock.Setup(fr => fr.ReadAllText(It.IsAny<string>())).Returns(validJson);

        // Act
        var result = _bookingRepository.GetBookingByType("DBL");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal("DBL", result.Data.RoomType);
    }

    [Fact]
    public void GetBookingByType_ShouldReturnNull_WhenRoomTypeDoesNotExist()
    {
        // Arrange
        var validJson = @"
        [
            {
                ""hotelId"": ""H1"",
                ""arrival"": ""20240901"",
                ""departure"": ""20240903"",
                ""roomType"": ""DBL"",
                ""roomRate"": ""Prepaid""
            }
        ]";
        _fileReaderMock.Setup(fr => fr.ReadAllText(It.IsAny<string>())).Returns(validJson);

        // Act
        var result = _bookingRepository.GetBookingByType("SGL");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Null(result.Data);
    }
}

