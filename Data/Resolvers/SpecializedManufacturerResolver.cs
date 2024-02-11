using AutoMapper;
using BlazorApp.Shared.Models;
using Data.Specialized.Models;

namespace Data.Resolvers
{
    public class SpecializedManufacturerResolver : IValueResolver<SpecializedModel, Model, Manufacturer>
    {
        public Manufacturer Resolve(SpecializedModel source, Model destination, Manufacturer destMember, ResolutionContext context)
        {
            return new Manufacturer("Specialized")
            {
                Id = new Guid("2c26eb22-8434-437e-8f1f-b75f0fe934af")
            };
        }
    }
}
