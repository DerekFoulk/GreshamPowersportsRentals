using Data.Factories;
using Data.Husqvarna.Models;
using Data.Husqvarna.Pages;
using Data.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;

namespace Data.Husqvarna.Services
{
    public class HusqvarnaBicyclesService
    {
        private readonly ILogger<HusqvarnaBicyclesService> _logger;
        private readonly WebDriverOptions _webDriverOptions;
        private readonly IWebDriverFactory _webDriverFactory;

        public HusqvarnaBicyclesService(ILogger<HusqvarnaBicyclesService> logger, IOptionsSnapshot<WebDriverOptions> optionsSnapshot, IWebDriverFactory webDriverFactory)
        {
            _logger = logger;
            _webDriverOptions = optionsSnapshot.Value;
            _webDriverFactory = webDriverFactory;
        }

        public List<HusqvarnaBicycleInfo> GetBicycleInfos()
        {
            _logger.LogTrace("Getting bikes from Husqvarna's website");

            var webDriver = _webDriverFactory.GetWebDriver<EdgeDriver>(
                TimeSpan.FromSeconds(_webDriverOptions.ImplicitWaitInSeconds), _webDriverOptions.Headless);

            var bicycleInfos = new List<HusqvarnaBicycleInfo>();

            try
            {
                var homePage = new HomePage(_logger, webDriver);

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

                    var modelPage = new ModelPage(_logger, webDriver, href, text);

                    var bicycleInfo = modelPage.GetBicycleInfo();
                    bicycleInfos.Add(bicycleInfo);
                }
            }
            finally
            {
                webDriver.Quit();
            }

            return bicycleInfos;
        }
    }
}
