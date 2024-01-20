using Data.Extensions;
using Data.Factories;
using Data.Specialized.Models;
using Data.Specialized.Pages;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using System.Reflection;
using System.Text;
using BlazorApp.Shared.Contexts;
using Data.Options;
using Data.Specialized.Contexts;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Data.Specialized.Services
{
    public class SpecializedBikesService : ISpecializedBikesService
    {
        private readonly ILogger<SpecializedBikesService> _logger;
        private readonly SpecializedContext _context;
        private readonly WebDriverOptions _webDriverOptions;
        private readonly IWebDriverFactory _webDriverFactory;

        public SpecializedBikesService(ILogger<SpecializedBikesService> logger, IOptionsSnapshot<WebDriverOptions> optionsSnapshot, SpecializedContext context, IWebDriverFactory webDriverFactory)
        {
            _logger = logger;
            _context = context;
            _webDriverOptions = optionsSnapshot.Value;
            _webDriverFactory = webDriverFactory;
        }

        public IEnumerable<Model> GetModels()
        {
            _logger.LogDebug("Getting bikes from Specialized's website");

            var models = new List<Model>();

            var webDriver = _webDriverFactory.GetWebDriver<EdgeDriver>(
                TimeSpan.FromSeconds(_webDriverOptions.ImplicitWaitInSeconds), _webDriverOptions.Headless);

            try
            {
                var bikesPage = new BikesPage(_logger, webDriver);

                var urls = bikesPage.GetBikeDetailUrlsAcrossPages().Distinct().ToList();

                _logger.LogDebug($"Found {urls.Count} bike details page URLs to scrape");

                foreach (var url in urls)
                {
                    var bikeDetails = GetBikeDetails(url, webDriver);

                    models.Add(bikeDetails);
                }
            }
            catch (Exception e)
            {
                webDriver.CaptureHtmlAndScreenshot(e, GetType(), MethodBase.GetCurrentMethod());

                _logger.LogError(e, "Failed to scrape bikes from Specialized's website");

                throw;
            }
            finally
            {
                webDriver.Quit();
            }

            _logger.LogInformation($"Scraped {models.Count} bikes from Specialized's website");

            _context.Models.UpdateRange(models);

            return models;
        }

        private bool TryGetBikeDetails(string url, IWebDriver webDriver, out Model? bikeDetails)
        {
            bikeDetails = null;

            _logger.LogDebug($"Trying to get bike details from '{url}'");

            try
            {
                bikeDetails = GetBikeDetails(url, webDriver);

                return true;
            }
            catch (Exception e)
            {
                //webDriver.CaptureHtmlAndScreenshot($"{DateTime.Now:yyyyMMddHHmmss}_{e.GetType().FullName}_{GetType().FullName}_{MethodBase.GetCurrentMethod()?.Name}");
                webDriver.CaptureHtmlAndScreenshot(e, GetType(), MethodBase.GetCurrentMethod());

                _logger.LogError(e, $"Failed to scrape bike details from '{url}'");

                return false;
            }
        }

        private Model GetBikeDetails(string url, IWebDriver webDriver)
        {
            _logger.LogDebug($"Getting bike details from '{url}'");

            _logger.LogTrace($"Navigating to '{url}'");

            webDriver.Navigate().GoToUrl(url);

            var bikeDetailsPage = new BikeDetailsPage(_logger, webDriver);
            var bikeDetails = bikeDetailsPage.GetBikeDetails();

            return bikeDetails;
        }
    }
}
