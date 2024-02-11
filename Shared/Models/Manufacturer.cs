using System;

namespace BlazorApp.Shared.Models
{
    public class Manufacturer
    {
        public Manufacturer(string name)
        {
            Name = name;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}
