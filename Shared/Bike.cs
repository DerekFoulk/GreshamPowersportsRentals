using System;
using BlazorApp.Shared.Extensions;

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

    public Model Model { get; set; }

    public override string ToString()
    {
        return $"{Model.Manufacturer.Name} {Model.Name} ({Size.GetDisplayName()})";
    }
}