using AutoMapper;
using BlazorApp.Shared.Models;
using Data.Husqvarna.Models;

namespace Data.Resolvers
{
    public class HusqvarnaManufacturerResolver : IValueResolver<HusqvarnaBicycleInfo, Model, Manufacturer>
    {
        public Manufacturer Resolve(HusqvarnaBicycleInfo source, Model destination, Manufacturer destMember, ResolutionContext context)
        {
            return new Manufacturer("Husqvarna")
            {
                Id = new Guid("4d8a3510-068b-4c9c-a7d8-40ca09a9a629")
            };
        }
    }
}
