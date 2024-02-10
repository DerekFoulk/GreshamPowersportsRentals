using Data.Pages;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using SeleniumExtras.PageObjects;

namespace Data.Husqvarna.Pages
{
    public class ModelsPage : BasePage<ModelsPage>
    {
        public ModelsPage(ILogger logger, IWebDriver webDriver) : base(logger, webDriver, "https://www.husqvarna-bicycles.com/en-us/models/ebikes.html")
        {
        }

        [FindsBy(How = How.CssSelector, Using = ".c-model-list__item h3 a")]
        public IList<IWebElement> ProductLinks { get; set; }

        public IEnumerable<string> GetBikeDetailUrls()
        {
            Logger.LogDebug("Getting bike detail page URLs");

            var urls = new List<string>();

            foreach (var productLink in ProductLinks)
            {
                var url = productLink.GetAttribute("href").Trim();

                urls.Add(url);
            }

            return urls;
        }
    }
}
