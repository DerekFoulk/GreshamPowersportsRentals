using System;

namespace BlazorApp.Shared;

public class Bike
{
    public Bike(string model)
    {
        Model = model;
    }

    public Guid Id { get; set; }

    public Manufacturer? Manufacturer { get; set; }

    public string Model { get; set; }

    public BikeSize Size { get; set; }

    public BikeType Type { get; set; }

    public Category? Category { get; set; }

    public string? Description { get; set; }

    public decimal PricePerHour { get; set; }

    public decimal PricePerDay => PricePerHour * 24;
}