using Data.Extensions;
using Data.Factories;
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

        public async Task<BikesResult> GetBikesAsync(int maxBikes, int minPage = 1)
        {
            var pageCount = CalculatePageSpan(maxBikes);
            var maxPage = (minPage + pageCount) - 1;

            return await GetBikesAsync(maxBikes, minPage, maxPage);
        }

        public async Task<BikesResult> GetBikesFromPagesAsync(int maxPage)
        {
            return await GetBikesAsync(null, null, maxPage);
        }

        public async Task<BikesResult> GetBikesFromPagesAsync(int maxPage, int minPage)
        {
            return await GetBikesAsync(null, minPage, maxPage);
        }

        public async Task<BikesResult> GetBikesAsync(int? maxBikes = null, int? minPage = null, int? maxPage = null)
        {
            _logger.LogDebug("Getting bikes from Specialized's website");

            var bikeDetailsPageResults = new List<BikeDetailsPageResult>();

            var webDriver = _webDriverFactory.GetWebDriver<EdgeDriver>(
                TimeSpan.FromSeconds(_webDriverOptions.ImplicitWaitInSeconds), _webDriverOptions.Headless);

            var bikesPagesResult = new BikesPagesResult();

            try
            {
                var bikesPage = new BikesPage(_logger, webDriver);

                bikesPagesResult = bikesPage.GetBikeDetailUrlsAcrossPages(maxPage, minPage);

                var urls = bikesPagesResult.BikesPageResults?.SelectMany(x => x.Urls).ToList();

                if (maxBikes is not null)
                    urls = urls?.Take((int)maxBikes).ToList();

                _logger.LogDebug($"Found {urls?.Count} bike details page URLs to scrape");

                if (urls?.Any() == true)
                {
                    await _context.Database.EnsureDeletedAsync();
                    await _context.Database.EnsureCreatedAsync();

                    foreach (var url in urls)
                    {
                        var bikeDetailsPageResult = GetBikeDetails(url, webDriver);

                        await SaveBikeDetailsAsync(bikeDetailsPageResult);

                        bikeDetailsPageResults.Add(bikeDetailsPageResult);
                    }
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

            _logger.LogInformation($"Scraped {bikeDetailsPageResults.Count} bikes from Specialized's website");

            var bikesResult = new BikesResult(bikesPagesResult, bikeDetailsPageResults)
            {
                MaxBikes = maxBikes,
                MinPage = minPage,
                MaxPage = maxPage
            };

            return bikesResult;
        }

        private async Task SaveBikeDetailsAsync(BikeDetailsPageResult bikeDetailsPageResult)
        {
            var model = bikeDetailsPageResult.Model;
            await _context.Models.AddAsync(model);

            await _context.SaveChangesAsync();
        }

        private static int CalculatePageSpan(int bikesCount)
        {
            const int bikesPerPage = 18;
            return (bikesCount + bikesPerPage - 1) / bikesPerPage;
        }

        private BikeDetailsPageResult GetBikeDetails(string url, IWebDriver webDriver)
        {
            _logger.LogDebug($"Getting bike details from '{url}'");

            _logger.LogTrace($"Navigating to '{url}'");

            webDriver.Navigate().GoToUrl(url);

            var bikeDetailsPage = new BikeDetailsPage(_logger, webDriver);
            var bikeDetailsPageResult = bikeDetailsPage.GetBikeDetails();

            return bikeDetailsPageResult;
        }
    }
}
