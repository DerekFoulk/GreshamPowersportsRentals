using Data.Factories;
using Data.Husqvarna.Contexts;
using Data.Husqvarna.Services;
using Data.Specialized.Contexts;
using Data.Specialized.Services;
using Microsoft.EntityFrameworkCore;
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

            services.AddDbContext<HusqvarnaContext>(options =>
                options.UseCosmos("https://cosmos-gprentals-dev-001.documents.azure.com:443/",
                    "FESOV0K6q0DTaHKp8ihWZQhRianqZGlkzzPpMQsplcVbUZLeGwqf5V0VbXNSxvVwhdNgZ9Wd9K3IACDbagBlAg==",
                    "Husqvarna")
            );

            services.AddDbContext<SpecializedContext>(options =>
                options.UseCosmos("https://cosmos-gprentals-dev-001.documents.azure.com:443/",
                    "FESOV0K6q0DTaHKp8ihWZQhRianqZGlkzzPpMQsplcVbUZLeGwqf5V0VbXNSxvVwhdNgZ9Wd9K3IACDbagBlAg==",
                    "Specialized")
            );

            return services;
        }
    }
}
