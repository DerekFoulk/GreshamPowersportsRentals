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

            var webDriver = _webDriverFactory.GetWebDriver<EdgeDriver>(TimeSpan.FromSeconds(5), false);

            try
            {
                var bikesPage = new BikesPage(_logger, webDriver);
                
                var urls = bikesPage.GetBikeDetailUrlsAcrossPages().Distinct();

                var scrapedBikesCount = 0;

                foreach (var url in urls)
                {
                    // TODO: Remove
                    if (scrapedBikesCount >= 2)
                        break;
                    
                    webDriver.Navigate().GoToUrl(url);

                    var bikeDetailsPage = new BikeDetailsPage(_logger, webDriver);
                    var bikeDetails = bikeDetailsPage.GetBikeDetails();

                    bikes.Add(bikeDetails);

                    scrapedBikesCount++;
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
