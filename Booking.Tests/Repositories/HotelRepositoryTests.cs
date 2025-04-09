using Booking.Helpers;
using Booking.Repositories;
using Moq;

namespace Booking.Tests.Repositories;
public class HotelRepositoryTests
{
    private readonly Mock<IFileReader> _fileReaderMock;
    private readonly HotelRepository _hotelRepository;

    public HotelRepositoryTests()
    {
        _fileReaderMock = new Mock<IFileReader>();
        _hotelRepository = new HotelRepository(_fileReaderMock.Object);
    }

    [Fact]
    public void GetHotels_ShouldReturnHotels_WhenJsonIsValid()
    {
        // Arrange
        var validJson = @"
        [
            {
                ""id"": ""H1"",
                ""name"": ""Hotel California"",
                ""roomTypes"": [
                    { ""code"": ""SGL"", ""size"": 1, ""description"": ""Single Room"", ""amenities"": [""WiFi"", ""TV""], ""features"": [""Non-smoking""] }
                ],
                ""rooms"": [
                    { ""roomType"": ""SGL"", ""roomId"": ""101"" }
                ]
            }
        ]";
        _fileReaderMock.Setup(fr => fr.ReadAllText(It.IsAny<string>())).Returns(validJson);

        // Act
        var result = _hotelRepository.GetHotels();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Single(result.Data);
        Assert.Equal("H1", result.Data.First().Id);
    }

    [Fact]
    public void GetHotels_ShouldReturnError_WhenJsonIsInvalid()
    {
        // Arrange
        var invalidJson = "Invalid JSON";
        _fileReaderMock.Setup(fr => fr.ReadAllText(It.IsAny<string>())).Returns(invalidJson);

        // Act
        var result = _hotelRepository.GetHotels();

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Error reading or deserializing the file", result.ErrorMessage);
    }

    [Fact]
    public void GetHotelById_ShouldReturnHotel_WhenHotelExists()
    {
        // Arrange
        var validJson = @"
        [
            { ""id"": ""H1"", ""name"": ""Hotel California"", ""roomTypes"": [], ""rooms"": [] },
            { ""id"": ""H2"", ""name"": ""Hotel Paradise"", ""roomTypes"": [], ""rooms"": [] }
        ]";
        _fileReaderMock.Setup(fr => fr.ReadAllText(It.IsAny<string>())).Returns(validJson);

        // Act
        var result = _hotelRepository.GetHotelById("H1");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal("H1", result.Data.Id);
    }

    [Fact]
    public void GetHotelById_ShouldReturnNull_WhenHotelDoesNotExist()
    {
        // Arrange
        var validJson = @"
        [
            { ""id"": ""H1"", ""name"": ""Hotel California"", ""roomTypes"": [], ""rooms"": [] }
        ]";
        _fileReaderMock.Setup(fr => fr.ReadAllText(It.IsAny<string>())).Returns(validJson);

        // Act
        var result = _hotelRepository.GetHotelById("H2");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Null(result.Data);
    }

    [Fact]
    public void GetHotelRooms_ShouldReturnAllRooms_WhenRoomTypeIsNull()
    {
        // Arrange
        var validJson = @"
        [
            {
                ""id"": ""H1"",
                ""name"": ""Hotel California"",
                ""roomTypes"": [],
                ""rooms"": [
                    { ""roomType"": ""SGL"", ""roomId"": ""101"" },
                    { ""roomType"": ""DBL"", ""roomId"": ""201"" }
                ]
            }
        ]";
        _fileReaderMock.Setup(fr => fr.ReadAllText(It.IsAny<string>())).Returns(validJson);

        // Act
        var result = _hotelRepository.GetHotelRooms("H1", null);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Data.Count());
    }

    [Fact]
    public void GetHotelRooms_ShouldFilterRooms_WhenRoomTypeIsProvided()
    {
        // Arrange
        var validJson = @"
        [
            {
                ""id"": ""H1"",
                ""name"": ""Hotel California"",
                ""roomTypes"": [],
                ""rooms"": [
                    { ""roomType"": ""SGL"", ""roomId"": ""101"" },
                    { ""roomType"": ""DBL"", ""roomId"": ""201"" }
                ]
            }
        ]";
        _fileReaderMock.Setup(fr => fr.ReadAllText(It.IsAny<string>())).Returns(validJson);

        // Act
        var result = _hotelRepository.GetHotelRooms("H1", "SGL");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(result.Data);
        Assert.Equal("101", result.Data.First().RoomId);
    }

    [Fact]
    public void GetHotelRooms_ShouldReturnError_WhenHotelDoesNotExist()
    {
        // Arrange
        var validJson = @"
        [
            { ""id"": ""H1"", ""name"": ""Hotel California"", ""roomTypes"": [], ""rooms"": [] }
        ]";
        _fileReaderMock.Setup(fr => fr.ReadAllText(It.IsAny<string>())).Returns(validJson);

        // Act
        var result = _hotelRepository.GetHotelRooms("H2", null);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Hotel with ID H2 not found.", result.ErrorMessage);
    }

    [Fact]
    public void GetHotelRooms_ShouldReturnError_WhenNoRoomsMatchRoomType()
    {
        // Arrange
        var validJson = @"
        [
            {
                ""id"": ""H1"",
                ""name"": ""Hotel California"",
                ""roomTypes"": [],
                ""rooms"": [
                    { ""roomType"": ""SGL"", ""roomId"": ""101"" }
                ]
            }
        ]";
        _fileReaderMock.Setup(fr => fr.ReadAllText(It.IsAny<string>())).Returns(validJson);

        // Act
        var result = _hotelRepository.GetHotelRooms("H1", "DBL");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("No rooms found for hotel with ID H1 and room type DBL.", result.ErrorMessage);
    }
}

