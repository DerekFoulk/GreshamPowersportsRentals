using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlazorApp.Shared
{
    public class Rental
    {
        [Required]
        public Guid Id { get; init; }

        [Required]
        public DateOnly StartDate { get; set; }

        [Required]
        public TimeOnly StartTime { get; set; }

        [Required]
        public DateOnly EndDate { get; set; }

        [Required]
        public TimeOnly EndTime { get; set; }

        [Required]
        public IEnumerable<Bike> Bikes { get; set; } = new List<Bike>();
    }
}
