using Data.Extensions;
using Data.Factories;
using Data.Husqvarna.Models;
using Data.Husqvarna.Pages;
using Data.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenQA.Selenium;
using System.Reflection;

namespace Data.Husqvarna.Services
{
    public class HusqvarnaBicyclesService : IHusqvarnaBicyclesService
    {
        private readonly ILogger<HusqvarnaBicyclesService> _logger;
        private readonly IWebDriver _webDriver;

        public HusqvarnaBicyclesService(ILogger<HusqvarnaBicyclesService> logger, IOptionsSnapshot<WebDriverOptions> optionsSnapshot, IWebDriverFactory webDriverFactory)
        {
            _logger = logger;
            var webDriverOptions = optionsSnapshot.Value;

            _webDriver = webDriverFactory.GetWebDriver(TimeSpan.FromSeconds(webDriverOptions.ImplicitWaitInSeconds), webDriverOptions.Headless);
        }

        public IEnumerable<HusqvarnaBicycleInfo> GetBicycleInfos()
        {
            _logger.LogTrace("Getting bikes from Husqvarna's website");

            var bicycleInfos = new List<HusqvarnaBicycleInfo>();

            try
            {
                var modelsPage = new ModelsPage(_logger, _webDriver);

                var urls = modelsPage.GetBikeDetailUrls()
                    .Distinct()
                    .ToList();

                foreach (var url in urls)
                {
                    _logger.LogDebug($"Getting details #{urls.IndexOf(url)} ({url})");

                    var bicycleInfo = GetBicycleInfo(url);

                    bicycleInfos.Add(bicycleInfo);
                }
            }
            catch (Exception e)
            {
                if (e is WebDriverException)
                    _webDriver.CaptureHtmlAndScreenshot(e, GetType(), MethodBase.GetCurrentMethod());

                _logger.LogError(e, "Failed to scrape bikes from Husqvarna's website");

                throw;
            }

            return bicycleInfos;
        }

        public HusqvarnaBicycleInfo GetBicycleInfo(string url)
        {
            _logger.LogDebug($"Getting bike details from '{url}'");

            var modelPage = new ModelPage(_logger, _webDriver, url);

            var bicycleInfo = modelPage.GetBicycleInfo();

            return bicycleInfo;
        }

        public void Dispose()
        {
            _webDriver.Quit();
        }
    }
}
