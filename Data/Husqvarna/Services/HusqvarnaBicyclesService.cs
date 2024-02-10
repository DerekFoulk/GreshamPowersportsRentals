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
    public class HusqvarnaBicyclesService : IDisposable
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
                var homePage = new HomePage(_logger, _webDriver);

                List<IWebElement> modelMenuItems;
                try
                {
                    modelMenuItems = homePage.MainMenuElement.GetModelMenuItems();
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Failed to get menu items in 'Models'");

                    throw;
                }

                _logger.LogTrace($"Scraped '{modelMenuItems.Count}' models from the menu");

                var targets = new List<(string Text, string Href)>();

                foreach (var modelMenuItem in modelMenuItems)
                {
                    var text = modelMenuItem.Text.Trim();
                    var href = modelMenuItem.GetAttribute("href").Trim();

                    targets.Add((text, href));
                }

                foreach (var (text, href) in targets)
                {
                    _logger.LogTrace($"Getting details for {text} from {href}");

                    var modelPage = new ModelPage(_logger, _webDriver, href, text);

                    var bicycleInfo = modelPage.GetBicycleInfo();
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

        public void Dispose()
        {
            _webDriver.Quit();
        }
    }
}
