using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BlazorApp.Shared.Models
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
        public List<Bike> Bikes { get; set; } = new();

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"Rental #{Id}");
            stringBuilder.AppendLine($"Starts: {StartDate.ToDateTime(StartTime)}");
            stringBuilder.AppendLine($"Ends: {EndDate.ToDateTime(EndTime)}");
            stringBuilder.AppendLine("Bikes:");

            foreach (var bike in Bikes)
                stringBuilder.AppendLine(bike.ToString());

            return stringBuilder.ToString();
        }
    }
}
