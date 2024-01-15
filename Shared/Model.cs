using System;
using System.Collections.Generic;

namespace BlazorApp.Shared
{
    public record Model(
        Guid Id,
        Manufacturer Manufacturer,
        string Name,
        BikeType Type,
        Category Category,
        string Description,
        decimal PricePerHour,
        decimal PricePerDay,
        decimal PricePerWeek,
        IEnumerable<Bike> Bikes
    );
}
