using System;
using System.ComponentModel.DataAnnotations;

namespace BlazorApp.Shared
{
    public class Entity
    {
        public Entity() { }

        [Key]
        public Guid Id { get; set; }
    }
}
