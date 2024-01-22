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

            services.AddDbContextFactory<HusqvarnaContext>(options =>
                options.UseCosmos("https://localhost:8081",
                    "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
                    "Husqvarna")
                //options.UseCosmos("https://cosmos-gprentals-dev-001.documents.azure.com:443/",
                //    "FESOV0K6q0DTaHKp8ihWZQhRianqZGlkzzPpMQsplcVbUZLeGwqf5V0VbXNSxvVwhdNgZ9Wd9K3IACDbagBlAg==",
                //    "Husqvarna")
            );

            services.AddDbContextFactory<SpecializedContext>(options =>
                options.UseCosmos("https://localhost:8081",
                    "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
                    "Specialized")
                //options.UseCosmos("https://cosmos-gprentals-dev-001.documents.azure.com:443/",
                //    "FESOV0K6q0DTaHKp8ihWZQhRianqZGlkzzPpMQsplcVbUZLeGwqf5V0VbXNSxvVwhdNgZ9Wd9K3IACDbagBlAg==",
                //    "Specialized")
            );

            return services;
        }
    }
}
