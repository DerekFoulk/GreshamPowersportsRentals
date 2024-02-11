using System;

namespace BlazorApp.Shared.Models
{
    public class Category
    {
        public Category(string name)
        {
            Name = name;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string? Image { get; set; }
    }
}
