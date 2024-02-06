using Data.Extensions;
using Data.Factories;
using Data.Specialized.Models;
using Data.Specialized.Pages;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using System.Reflection;
using System.Text;
using Data.Options;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;

namespace Data.Specialized.Services
{
    public class SpecializedBikesService
    {
        private readonly ILogger<SpecializedBikesService> _logger;
        private readonly WebDriverOptions _webDriverOptions;
        private readonly IWebDriverFactory _webDriverFactory;

        public SpecializedBikesService(ILogger<SpecializedBikesService> logger, IOptionsSnapshot<WebDriverOptions> optionsSnapshot, IWebDriverFactory webDriverFactory)
        {
            _logger = logger;
            _webDriverOptions = optionsSnapshot.Value;
            _webDriverFactory = webDriverFactory;
        }

        public async Task<IEnumerable<Model>> GetModelsAsync(int? maxPage = null, int? minPage = null)
        {
            _logger.LogDebug("Getting bikes from Specialized's website");

            var models = new List<Model>();

            var webDriver = _webDriverFactory.GetWebDriver<FirefoxDriver>(
                TimeSpan.FromSeconds(_webDriverOptions.ImplicitWaitInSeconds), _webDriverOptions.Headless);

            try
            {
                var bikesPage = new BikesPage(_logger, webDriver);

                bikesPage.GoToPage(minPage ?? 1);

                var urls = bikesPage.GetBikeDetailUrlsAcrossPages(maxPage, minPage).Distinct().ToList();

                _logger.LogDebug($"Found {urls.Count} bike details page URLs to scrape");

                foreach (var url in urls)
                {
                    _logger.LogDebug($"Getting bike details #{urls.IndexOf(url)} ({url})");

                    var bikeDetails = GetBikeDetails(url, webDriver);

                    models.Add(bikeDetails);
                }

                _logger.LogDebug("Finished getting bike details");
            }
            catch (Exception e)
            {
                if (e is WebDriverException)
                    webDriver.CaptureHtmlAndScreenshot(e, GetType(), MethodBase.GetCurrentMethod());

                _logger.LogError(e, "Failed to scrape bikes from Specialized's website");

                throw;
            }
            finally
            {
                webDriver.Quit();
            }

            _logger.LogInformation($"Scraped {models.Count} bikes from Specialized's website");

            await SaveBikesAsJsonAsync(models);

            return models;
        }

        private async Task SaveBikesAsJsonAsync(List<Model> bikes)
        {
            const string path = @"R:\GreshamPowersportsRentals\Api\Specialized.json";

            _logger.LogDebug($"Saving bikes to JSON ('{path}')");

            //await using var fileStream = File.Create("Specialized.json");
            //await JsonSerializer.SerializeAsync(fileStream, bikes);

            var json = JsonConvert.SerializeObject(bikes);
            await File.WriteAllTextAsync(path, json, Encoding.UTF8);
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
