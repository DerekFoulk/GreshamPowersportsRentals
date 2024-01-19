using Data.Factories;
using Data.Husqvarna.Services;
using Data.Specialized.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Data.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddScrapers(this IServiceCollection services)
        {
            services.AddSingleton<IWebDriverFactory, WebDriverFactory>();
            services.AddScoped<ISpecializedBikesService, SpecializedBikesService>();
            services.AddScoped<IHusqvarnaBicyclesService, HusqvarnaBicyclesService>();

            return services;
        }
    }
}
