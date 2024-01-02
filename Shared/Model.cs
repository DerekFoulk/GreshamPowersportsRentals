using System;
using System.Collections.Generic;

namespace BlazorApp.Shared
{
    public class Model
    {
        public Model(string name, Category category, Manufacturer manufacturer)
        {
            Name = name;
            Category = category;
            Manufacturer = manufacturer;
        }

        public Guid Id { get; set; }

        public Manufacturer Manufacturer { get; set; }

        public string Name { get; set; }

        public BikeType Type { get; set; }

        public Category Category { get; set; }

        public string? Description { get; set; }

        public decimal PricePerHour { get; set; }

        public decimal PricePerDay => PricePerHour * 24;

        public IEnumerable<Bike> Bikes { get; set; } = new List<Bike>();
    }
}
