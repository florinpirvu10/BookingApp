﻿namespace Booking.Entities;

public class Response<T>
{
    public Response() { }

    public Response(T data)
    {
        Data = data;
    }

    public T Data { get; set; }
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
}
