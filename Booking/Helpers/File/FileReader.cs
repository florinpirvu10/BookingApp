﻿namespace Booking.Helpers;

public class FileReader : IFileReader
{
    public string ReadAllText(string path)
    {
        return File.ReadAllText(path);
    }
}

