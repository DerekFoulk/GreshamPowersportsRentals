using System;

namespace BlazorApp.Shared;

public class Bike
{
    public Bike(BikeSize size)
    {
        Size = size;
    }

    public Guid Id { get; set; }

    public BikeSize Size { get; set; }

    public bool IsAvailable { get; set; }
}