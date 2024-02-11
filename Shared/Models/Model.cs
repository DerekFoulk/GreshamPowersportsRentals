using System;
using System.Collections.Generic;

namespace BlazorApp.Shared.Models
{
    public class Model
    {
        public Guid Id { get; set; }
        public required Manufacturer Manufacturer { get; set; }
        public required string Name { get; set; }
        public string? Url { get; set; }
        public BikeType Type { get; set; }
        public required Category Category { get; set; }
        public string? Description { get; set; }
        public decimal PricePerHour { get; set; }
        public decimal PricePerDay { get; set; }
        public decimal PricePerWeek { get; set; }
        public required IEnumerable<Bike> Bikes { get; set; }
    }
}
