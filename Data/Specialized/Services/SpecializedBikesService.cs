using Data.Extensions;
using Data.Factories;
using Data.Specialized.Models;
using Data.Specialized.Pages;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using System.Reflection;
using Data.Options;
using Data.Specialized.Contexts;
using Data.Specialized.Entities;
using Microsoft.Extensions.Options;

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

        public BikesResult GetBikes(int maxBikes, int minPage = 1)
        {
            var pageCount = CalculatePageSpan(maxBikes);
            var maxPage = (minPage + pageCount) - 1;

            return GetBikes(maxBikes, minPage, maxPage);
        }

        public BikesResult GetBikesFromPages(int maxPage)
        {
            return GetBikes(null, null, maxPage);
        }

        public BikesResult GetBikesFromPages(int maxPage, int minPage)
        {
            return GetBikes(null, minPage, maxPage);
        }

        public BikesResult GetBikes(int? maxBikes = null, int? minPage = null, int? maxPage = null)
        {
            _logger.LogDebug("Getting bikes from Specialized's website");

            var models = new List<Model>();

            var webDriver = _webDriverFactory.GetWebDriver<EdgeDriver>(
                TimeSpan.FromSeconds(_webDriverOptions.ImplicitWaitInSeconds), _webDriverOptions.Headless);

            var exceptions = new List<Exception>();

            try
            {
                var bikesPage = new BikesPage(_logger, webDriver);

                var urls = bikesPage.GetBikeDetailUrlsAcrossPages(maxPage, minPage).Distinct().ToList();

                if (maxBikes is not null)
                    urls = urls.Take((int)maxBikes).ToList();

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

                exceptions.Add(e);
            }
            finally
            {
                webDriver.Quit();
            }

            _logger.LogInformation($"Scraped {models.Count} bikes from Specialized's website");

            try
            {
                _context.Database.EnsureDeleted();
                _context.Database.EnsureCreated();

                _context.Models.UpdateRange(models);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to update and save models to Cosmos DB");

                exceptions.Add(e);
            }

            var bikesResult = new BikesResult(bikesPagesResult, bikeDetailsPageResults)
            {
                Exceptions = exceptions
            };

            return bikesResult;
        }

        private static int CalculatePageSpan(int bikesCount)
        {
            const int bikesPerPage = 18;
            return (bikesCount + bikesPerPage - 1) / bikesPerPage;
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
