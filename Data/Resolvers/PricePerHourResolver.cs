using AutoMapper;
using BlazorApp.Shared.Models;

namespace Data.Resolvers
{
    public class PricePerHourResolver : IMemberValueResolver<object, Model, decimal, decimal>
    {
        public decimal Resolve(object source, Model destination, decimal sourceMsrp, decimal destPricePerHour, ResolutionContext context)
        {
            var msrp = sourceMsrp;
            var totalHoursInAYear = (decimal)TimeSpan.FromDays(90).TotalHours;
            var pricePerHour = msrp / totalHoursInAYear;

            return pricePerHour;
        }
    }
}
