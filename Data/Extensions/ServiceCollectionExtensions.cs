using Data.Factories;
using Microsoft.Extensions.DependencyInjection;

namespace Data.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWebDriverFactory(this IServiceCollection services)
        {
            services.AddSingleton<IWebDriverFactory, WebDriverFactory>();

            return services;
        }
    }
}
