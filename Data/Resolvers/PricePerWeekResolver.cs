using AutoMapper;
using BlazorApp.Shared.Models;

namespace Data.Resolvers
{
    public class PricePerWeekResolver : IValueResolver<object, Model, decimal>
    {
        public decimal Resolve(object source, Model destination, decimal destMember, ResolutionContext context)
        {
            var pricePerDay = destination.PricePerDay;
            var pricePerWeek = pricePerDay * 7;
            pricePerWeek *= 0.75M;
            pricePerWeek = Math.Round(pricePerWeek / 5) * 5;

            return pricePerWeek;
        }
    }
}
