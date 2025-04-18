﻿using Booking.Entities;
using Booking.Repositories;

namespace Booking.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IHotelRepository _hotelRepository;

    public BookingService(IBookingRepository bookingRepository, IHotelRepository hotelRepository)
    {
        _bookingRepository = bookingRepository;
        _hotelRepository = hotelRepository;
    }

    public Response<string> CheckRoomAvailability(string hotelId, string roomType, DateTime start, DateTime end)
    {
        var response = new Response<string>();

        var bookings = _bookingRepository.GetBookings();
        if (!bookings.IsSuccess)
        {
            response.ErrorMessage = bookings.ErrorMessage;
            return response;
        }

        var rooms = _hotelRepository.GetHotelRooms(hotelId, roomType);
        if (!rooms.IsSuccess)
        {
            response.ErrorMessage = rooms.ErrorMessage;
            return response;
        }

        var numberOfBookings = CountBookings(bookings.Data, hotelId, roomType, start, end);

        var typeRooms = rooms.Data
            .Where(room => room.RoomType == roomType)
            .Count();

        int availableRooms = typeRooms - numberOfBookings;

        switch (availableRooms)
        {
            case < 0:
                response.Data = $"Overbooked by: {availableRooms}";
                break;
            case > 0:
                response.Data = $"Available rooms: {availableRooms}";
                break;
            default:
                response.Data = "No rooms available for the specified criteria.";
                break;
        }

        response.IsSuccess = true;
        return response;
    }

    public Response<string> GetRoomTypesForPeople(string hotelId, DateTime start, DateTime end, int numberOfPeople)
    {
        var response = new Response<string>();

        var hotel = _hotelRepository.GetHotelById(hotelId);
        if (!hotel.IsSuccess || hotel.Data == null)
        {
            response.ErrorMessage = hotel.ErrorMessage ?? $"Hotel with ID {hotelId} not found.";
            return response;
        }

        var bookings = _bookingRepository.GetBookings();
        if (!bookings.IsSuccess)
        {
            response.ErrorMessage = bookings.ErrorMessage;
            return response;
        }

        var availableRoomTypes = GetAvailableRoomTypes(bookings.Data, hotel.Data, start, end);
        var allocatedRooms = AllocateRooms(availableRoomTypes, numberOfPeople);


        if (allocatedRooms == null)
        {
            response.ErrorMessage = "Allocation not possible with the available rooms.";
            return response;
        }

        response.Data = $"{hotel.Data.Name}: {string.Join(", ", allocatedRooms)}";
        response.IsSuccess = true;
        return response;
    }

    protected static IEnumerable<AvailableRoomType> GetAvailableRoomTypes(IEnumerable<BookingEntity> bookings, HotelEntity hotel, DateTime start, DateTime end)
    {
        var conflictingRoomTypes = bookings
            .Where(b => b.HotelId == hotel.Id)
            .Where(b =>
                DateTime.TryParseExact(b.Arrival, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime arrival) &&
                DateTime.TryParseExact(b.Departure, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime departure) &&
                !(departure <= start || arrival >= end))
            .Select(b => b.RoomType);

        var availableRoomTypes = hotel.Rooms
            .GroupBy(room => room.RoomType)
            .Select(group => new AvailableRoomType
            {
                RoomType = group.Key,
                Count = group.Count() - conflictingRoomTypes.Where(x => x == group.Key).Count(),
                Capacity = hotel.RoomTypes.First(rt => rt.Code == group.Key).Size
            })
            .Where(x => x.Count > 0)
            .OrderByDescending(rt => rt.Capacity)
            .ToList();

        return availableRoomTypes;
    }

    protected static IEnumerable<string>? AllocateRooms(IEnumerable<AvailableRoomType> rooms, int numberOfPeople)
    {
        var allocatedRooms = new List<string>();
        int remainingPeople = numberOfPeople;

        foreach (var room in rooms.OrderByDescending(r => r.Capacity))
        {
            while (remainingPeople > 0 && room.Count > 0)
            {
                if (remainingPeople >= room.Capacity)
                {
                    allocatedRooms.Add(room.RoomType);
                    remainingPeople -= room.Capacity;
                }
                else
                {
                    allocatedRooms.Add(room.RoomType + "!");
                    remainingPeople = 0;
                }
                room.Count--;
            }

            if (remainingPeople == 0)
                break;
        }

        if (remainingPeople > 0)
        {
            return null;
        }

        return allocatedRooms;
    }

    protected static int CountBookings(IEnumerable<BookingEntity> bookings, string hotelId, string roomType, DateTime start, DateTime end)
    {
        return bookings
             .Where(b => b.HotelId == hotelId && b.RoomType == roomType)
             .Count(b =>
                 DateTime.TryParseExact(b.Arrival, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime arrival) &&
                 DateTime.TryParseExact(b.Departure, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime departure) &&
                 !(departure <= start || arrival >= end));
    }
}
