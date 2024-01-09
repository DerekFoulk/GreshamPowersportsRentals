using System.Diagnostics;
using Data.Husqvarna.Pages.Shared;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.PageObjects;

namespace Data.Specialized.Pages
{
    public class BikesPage : Page<BikesPage>
    {
        public BikesPage(ILogger logger, IWebDriver webDriver) : base(logger, webDriver)
        {
            GoToPage();
        }

        [FindsBy(How = How.CssSelector, Using = "[data-component=\"product-tile\"]")]
        public IList<IWebElement> Products { get; set; }

        public void GoToPage(int pageNumber = 1)
        {
            Logger.LogTrace("Navigating to the 'Bikes' page");

            WebDriver.Navigate().GoToUrl($"https://www.specialized.com/us/en/c/bikes?group=Bikes&group=E-Bikes&page={pageNumber}");
        }

        public List<string> GetBikeDetailUrlsAcrossPages()
        {
            Logger.LogTrace("Getting bikes from the 'Bikes' page");

            Logger.LogTrace("Gathering bike detail page URLs");

            var bikeDetailsPagesToScrape = new List<string>();

            var stopwatch = Stopwatch.StartNew();

            var currentPageNumber = GetCurrentPageNumberFromUrl();

            // TODO: Figure out why the first page is scraped twice
            while (!IsLastPage() && currentPageNumber == 1)
            {
                //var currentPageNumber = GetCurrentPageNumberFromUrl();

                var urls = GetBikeDetailUrlsFromPage(stopwatch, currentPageNumber);

                bikeDetailsPagesToScrape.AddRange(urls);

                //var nextPageNumber = GoToNextPage(currentPageNumber);

                GoToPage(currentPageNumber++);
            }

            return bikeDetailsPagesToScrape;
        }

        private int GoToNextPage(int currentPageNumber)
        {
            var currentUrl = WebDriver.Url;

            Logger.LogInformation($"Current URL: '{currentUrl}'");

            WebDriver.FindElement(By.CssSelector("button.sc-b650d9bf-0.cAXHmd")).Click();

            new WebDriverWait(WebDriver, TimeSpan.FromSeconds(10))
                .Until(x =>
                {
                    var url = x.Url;
                    
                    Logger.LogInformation($"New URL: '{url}'");

                    var isUrlDifferent = !url.Equals(currentUrl, StringComparison.OrdinalIgnoreCase);

                    Logger.LogInformation($"Is URL Different? '{(isUrlDifferent ? "Yes" : "No")}'");

                    return isUrlDifferent;
                });

            // Required to refresh the page object
            PageFactory.InitElements(WebDriver, this);

            var newPageNumber = GetCurrentPageNumberFromUrl();

            Logger.LogTrace($"New page number: {newPageNumber}");

            if (newPageNumber <= currentPageNumber)
                throw new Exception($"'{nameof(newPageNumber)}' must be greater than '{nameof(currentPageNumber)}'");

            return newPageNumber;
        }

        private IEnumerable<string> GetBikeDetailUrlsFromPage(Stopwatch stopwatch, int pageNumber)
        {

            var bikeDetailsPagesToScrape = new List<string>();

            if (stopwatch.Elapsed >= TimeSpan.FromMinutes(3))
            {
                stopwatch.Stop();

                throw new TimeoutException($"Gatering of bike detail page URLs timed out after '{stopwatch.Elapsed}'");
            }

            Logger.LogTrace($"Gathering bike detail page URLs from page #{pageNumber}");

            foreach (var product in Products)
            {
                var url = product.FindElement(By.CssSelector("a")).GetAttribute("href").Trim();
                var name = product.FindElement(By.CssSelector("h3")).Text.Trim();

                Logger.LogTrace($"Adding '{name}' ({url}) #{pageNumber}");
                
                bikeDetailsPagesToScrape.Add(url);
            }

            return bikeDetailsPagesToScrape;
        }

        private int GetCurrentPageNumberFromUrl()
        {
            Logger.LogTrace("Getting the current page number");

            var pageNumber = int.Parse(WebDriver.Url.Split("page=").Last());

            Logger.LogTrace($"Current page number: {pageNumber}");

            return pageNumber;
        }

        private bool IsLastPage()
        {
            Logger.LogTrace("Checking if the current page is the last page");

            try
            {
                WebDriver.FindElement(By.CssSelector("button.sc-b650d9bf-0.cAXHmd"));

                Logger.LogTrace("Is not the last page");

                return false;
            }
            catch (NoSuchElementException)
            {
                //WebDriver.FindElement(By.CssSelector("button.sc-b650d9bf-0.cKfDgr"));

                Logger.LogTrace("Is the last page");

                return true;
            }
        }
    }
}
