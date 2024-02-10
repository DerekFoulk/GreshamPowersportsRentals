using Data.Extensions;
using Data.Factories;
using Data.Specialized.Models;
using Data.Specialized.Pages;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using System.Reflection;
using System.Text;
using Data.Options;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Data.Specialized.Services
{
    public class SpecializedBikesService : IDisposable
    {
        private readonly ILogger<SpecializedBikesService> _logger;
        private readonly IWebDriver _webDriver;

        public SpecializedBikesService(ILogger<SpecializedBikesService> logger, IOptionsSnapshot<WebDriverOptions> optionsSnapshot, IWebDriverFactory webDriverFactory)
        {
            _logger = logger;
            var webDriverOptions = optionsSnapshot.Value;

            _webDriver = webDriverFactory.GetWebDriver(TimeSpan.FromSeconds(webDriverOptions.ImplicitWaitInSeconds), webDriverOptions.Headless);
        }

        public async Task<IEnumerable<SpecializedModel>> GetModelsAsync(int? maxPage = null, int? minPage = null)
        {
            _logger.LogDebug("Getting bikes from Specialized's website");

            var models = new List<SpecializedModel>();

            try
            {
                var bikesPage = new BikesPage(_logger, _webDriver);

                bikesPage.GoToPage(minPage ?? 1);

                var urls = bikesPage.GetBikeDetailUrlsAcrossPages(maxPage, minPage)
                    .Distinct()
                    .ToList();

                _logger.LogDebug($"Found {urls.Count} bike details page URLs to scrape");

                foreach (var url in urls)
                {
                    _logger.LogDebug($"Getting bike details #{urls.IndexOf(url)} ({url})");

                    var model = GetModel(url);

                    models.Add(model);
                }

                _logger.LogDebug("Finished getting bike details");
            }
            catch (Exception e)
            {
                if (e is WebDriverException)
                    _webDriver.CaptureHtmlAndScreenshot(e, GetType(), MethodBase.GetCurrentMethod());

                _logger.LogError(e, "Failed to scrape bikes from Specialized's website");

                throw;
            }

            _logger.LogInformation($"Scraped {models.Count} bikes from Specialized's website");

            await SaveBikesAsJsonAsync(models);

            return models;
        }

        public SpecializedModel GetModel(string url)
        {
            _logger.LogDebug($"Getting bike details from '{url}'");

            var bikeDetailsPage = new BikeDetailsPage(_logger, _webDriver, url);
            var bikeDetails = bikeDetailsPage.GetBikeDetails();

            return bikeDetails;
        }

        private async Task SaveBikesAsJsonAsync(List<SpecializedModel> bikes)
        {
            const string path = @"R:\GreshamPowersportsRentals\Api\Specialized.json";

            _logger.LogDebug($"Saving bikes to JSON ('{path}')");

            //await using var fileStream = File.Create("Specialized.json");
            //await JsonSerializer.SerializeAsync(fileStream, bikes);

            var json = JsonConvert.SerializeObject(bikes);
            await File.WriteAllTextAsync(path, json, Encoding.UTF8);
        }

        public void Dispose()
        {
            _webDriver.Quit();
        }
    }
}
