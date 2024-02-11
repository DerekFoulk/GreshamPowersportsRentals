using Data.Factories;
using Data.Husqvarna.Services;
using Data.Profiles;
using Data.Services;
using Data.Specialized.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Data.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWebDriverFactory(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MappingProfile));

            services.AddSingleton<IWebDriverFactory, WebDriverFactory>();

            services.AddScoped<ISpecializedBikesService, SpecializedBikesService>();
            services.AddScoped<IHusqvarnaBicyclesService, HusqvarnaBicyclesService>();
            services.AddScoped<IModelsService, ModelsService>();

            return services;
        }
    }
}
