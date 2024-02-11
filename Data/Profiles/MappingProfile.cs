using AutoMapper;
using BlazorApp.Shared.Models;
using Data.Husqvarna.Models;
using Data.Resolvers;
using Data.Specialized.Models;

namespace Data.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<SpecializedModel, Model>()
                .ForMember(dest => dest.Manufacturer, opt => opt.MapFrom<SpecializedManufacturerResolver>());
            CreateMap<HusqvarnaBicycleInfo, Model>()
                .ForMember(dest => dest.Manufacturer, opt => opt.MapFrom<HusqvarnaManufacturerResolver>())
                .ForMember(dest => dest.PricePerHour, opt => opt.MapFrom<PricePerHourResolver, decimal>(src => src.Msrp))
                .ForMember(dest => dest.PricePerDay, opt => opt.MapFrom<PricePerDayResolver>())
                .ForMember(dest => dest.PricePerWeek, opt => opt.MapFrom<PricePerWeekResolver>());
        }
    }
}
