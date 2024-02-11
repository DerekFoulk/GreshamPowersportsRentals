using AutoMapper;
using BlazorApp.Shared.Models;

namespace Data.Resolvers
{
    public class PricePerDayResolver : IValueResolver<object, Model, decimal>
    {
        public decimal Resolve(object source, Model destination, decimal destMember, ResolutionContext context)
        {
            var pricePerHour = destination.PricePerHour;
            var pricePerDay = pricePerHour * 24;
            pricePerDay = Math.Round(pricePerDay / 5) * 5;

            return pricePerDay;
        }
    }
}
