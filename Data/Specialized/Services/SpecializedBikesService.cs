using Data.Factories;
using Data.Specialized.Models;
using Data.Specialized.Pages;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium.Edge;

namespace Data.Specialized.Services
{
    public class SpecializedBikesService
    {
        private readonly ILogger<SpecializedBikesService> _logger;
        private readonly IWebDriverFactory _webDriverFactory;

        public SpecializedBikesService(ILogger<SpecializedBikesService> logger, IWebDriverFactory webDriverFactory)
        {
            _logger = logger;
            _webDriverFactory = webDriverFactory;
        }

        public IEnumerable<Model> GetBikes()
        {
            _logger.LogTrace("Getting bikes from Specialized's website");

            var bikes = new List<Model>();

            var webDriver = _webDriverFactory.GetWebDriver<EdgeDriver>(TimeSpan.FromSeconds(5), true);

            try
            {
                var bikesPage = new BikesPage(_logger, webDriver);
                
                var urls = bikesPage.GetBikeDetailUrlsAcrossPages().Distinct().ToList();

                foreach (var url in urls.Where(x => urls.IndexOf(x) == 1))
                {
                    webDriver.Navigate().GoToUrl(url);

                    var bikeDetailsPage = new BikeDetailsPage(_logger, webDriver);
                    var bikeDetails = bikeDetailsPage.GetBikeDetails();

                    bikes.Add(bikeDetails);
                }
            }
            finally
            {
                webDriver.Quit();
            }

            return bikes;
        }
    }
}
