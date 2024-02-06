using System;
using System.Collections.Generic;

namespace BlazorApp.Shared;

public class Bike
{
    public Bike(Model model, string size, string color, IEnumerable<string> images)
    {
        Model = model;
        Size = size;
        Color = color;
        Images = images;
    }

    public Guid Id { get; set; }

    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public Model Model { get; set; }

    public string Size { get; set; }
    
    public string Color { get; set; }
    
    public IEnumerable<string> Images { get; set; }
    
    public bool IsAvailable { get; set; }

    public override string ToString()
    {
        return $"{Model?.Manufacturer?.Name} {Model?.Name} ({Size})";
    }
}