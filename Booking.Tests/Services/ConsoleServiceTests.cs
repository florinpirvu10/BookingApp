using Booking.Services;
using Booking.Entities;
using Moq;
using Xunit;
using System;
using System.IO;

namespace Booking.Tests.Services
{
    public class ConsoleServiceTests
    {
        private readonly Mock<IBookingService> _bookingServiceMock;
        private readonly ConsoleService _consoleService;

        public ConsoleServiceTests()
        {
            _bookingServiceMock = new Mock<IBookingService>();
            _consoleService = new ConsoleService(_bookingServiceMock.Object);
        }

        [Fact]
        public void AvailabilityScenario_ShouldPrintError_WhenCommandFormatIsInvalid()
        {
            // Arrange
            var invalidInput = "Availability(H1, 20240901)";

            using var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            // Act
            _consoleService.AvailabilityScenario(invalidInput);

            // Assert
            Assert.Contains("Invalid command. Format: Availability(<HotelID>, <Date|DateRange>, <RoomType>)", consoleOutput.ToString());
        }

        [Fact]
        public void AvailabilityScenario_ShouldPrintError_WhenDateFormatIsInvalid()
        {
            // Arrange
            var invalidInput = "Availability(H1, 2024-09-01, DBL)";

            using var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            // Act
            _consoleService.AvailabilityScenario(invalidInput);

            // Assert
            Assert.Contains("Invalid date format. Use YYYYMMDD.", consoleOutput.ToString());
        }

        [Fact]
        public void AvailabilityScenario_ShouldPrintResponse_WhenServiceReturnsSuccess()
        {
            // Arrange
            var validInput = "Availability(H1, 20240901, DBL)";
            _bookingServiceMock.Setup(service => service.CheckRoomAvailability("H1", "DBL", It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(new Response<string> { IsSuccess = true, Data = "Available rooms: 201, 202" });

            using var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            // Act
            _consoleService.AvailabilityScenario(validInput);

            // Assert
            Assert.Contains("Available rooms: 201, 202", consoleOutput.ToString());
        }

        [Fact]
        public void AvailabilityScenario_ShouldPrintError_WhenServiceReturnsFailure()
        {
            // Arrange
            var validInput = "Availability(H1, 20240901, DBL)";
            _bookingServiceMock.Setup(service => service.CheckRoomAvailability("H1", "DBL", It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(new Response<string> { IsSuccess = false, ErrorMessage = "No rooms available." });

            using var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            // Act
            _consoleService.AvailabilityScenario(validInput);

            // Assert
            Assert.Contains("No rooms available.", consoleOutput.ToString());
        }

        [Fact]
        public void RoomTypesScenario_ShouldPrintError_WhenCommandFormatIsInvalid()
        {
            // Arrange
            var invalidInput = "RoomTypes(H1, 20240901)";

            using var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            // Act
            _consoleService.RoomTypesScenario(invalidInput);

            // Assert
            Assert.Contains("Invalid command. Format: RoomTypes(<HotelID>, <Date|DateRange>, <NumberOfPeople>)", consoleOutput.ToString());
        }

        [Fact]
        public void RoomTypesScenario_ShouldPrintError_WhenNumberOfPeopleIsInvalid()
        {
            // Arrange
            var invalidInput = "RoomTypes(H1, 20240901, -5)";

            using var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            // Act
            _consoleService.RoomTypesScenario(invalidInput);

            // Assert
            Assert.Contains("Invalid number of people. It must be a positive integer.", consoleOutput.ToString());
        }

        [Fact]
        public void RoomTypesScenario_ShouldPrintResponse_WhenServiceReturnsSuccess()
        {
            // Arrange
            var validInput = "RoomTypes(H1, 20240901, 3)";
            _bookingServiceMock.Setup(service => service.GetRoomTypesForPeople("H1", It.IsAny<DateTime>(), It.IsAny<DateTime>(), 3))
                .Returns(new Response<string> { IsSuccess = true, Data = "H1: DBL, SGL" });

            using var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            // Act
            _consoleService.RoomTypesScenario(validInput);

            // Assert
            Assert.Contains("H1: DBL, SGL", consoleOutput.ToString());
        }

        [Fact]
        public void RoomTypesScenario_ShouldPrintError_WhenServiceReturnsFailure()
        {
            // Arrange
            var validInput = "RoomTypes(H1, 20240901, 3)";
            _bookingServiceMock.Setup(service => service.GetRoomTypesForPeople("H1", It.IsAny<DateTime>(), It.IsAny<DateTime>(), 3))
                .Returns(new Response<string> { IsSuccess = false, ErrorMessage = "Allocation not possible." });

            using var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            // Act
            _consoleService.RoomTypesScenario(validInput);

            // Assert
            Assert.Contains("Allocation not possible.", consoleOutput.ToString());
        }
    }
}

