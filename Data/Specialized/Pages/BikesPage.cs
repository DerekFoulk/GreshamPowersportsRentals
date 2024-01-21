using System.Diagnostics;
using System.Text;
using AngleSharp.Common;
using Data.Husqvarna.Pages.Shared;
using Data.Specialized.Entities;
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
            var url = $"https://www.specialized.com/us/en/c/bikes?group=Bikes&group=E-Bikes&page={pageNumber}";
            
            Logger.LogTrace($"Navigating to the 'Bikes' page #{pageNumber} ('{url}')");

            WebDriver.Navigate().GoToUrl(url);
        }

        public BikesPagesResult GetBikeDetailUrlsAcrossPages(int? maxPage = null, int? minPage = null)
        {
            Logger.LogDebug("Getting bike detail page URLs");

            var currentPageNumber = GetCurrentPageNumberFromUrl();

            var stopwatch = Stopwatch.StartNew();

            var bikesPageResults = new List<BikesPageResult>();

            while (IsPageWithinRange(currentPageNumber, maxPage, minPage) && !IsLastPage())
            {
                if (stopwatch.Elapsed >= TimeSpan.FromMinutes(3))
                {
                    stopwatch.Stop();

                    throw new TimeoutException($"Getting bike detail page URLs timed out after '{stopwatch.Elapsed}'");
                }

                var urls = GetBikeDetailUrlsFromPage(currentPageNumber);

                var bikesPageResult = new BikesPageResult(WebDriver.Url, currentPageNumber, urls);

                bikesPageResults.Add(bikesPageResult);

                var nextPage = currentPageNumber + 1;

                if (IsPageWithinRange(nextPage, maxPage, minPage))
                    GoToPage(nextPage);

                currentPageNumber++;
            }

            var bikesPagesResult = new BikesPagesResult()
            {
                BikesPageResults = bikesPageResults,
                MinPage = minPage,
                MaxPage = maxPage
            };

            return bikesPagesResult;
        }

        private bool IsPageWithinRange(int pageNumber, int? maxPage = null, int? minPage = null)
        {
            var conditions = new List<bool>();

            if (maxPage is not null)
                conditions.Add(pageNumber <= maxPage);

            if (minPage is not null)
                conditions.Add(pageNumber >= minPage);

            var isPageWithinRange = conditions.All(x => x);

            return isPageWithinRange;
        }

        private int GoToNextPage(int currentPageNumber)
        {
            var currentUrl = WebDriver.Url;

            Logger.LogDebug($"Current URL: '{currentUrl}'");

            WebDriver.FindElement(By.CssSelector("button.sc-b650d9bf-0.cAXHmd")).Click();

            new WebDriverWait(WebDriver, TimeSpan.FromSeconds(10))
                .Until(x =>
                {
                    var url = x.Url;
                    
                    Logger.LogTrace($"New URL: '{url}'");

                    var isUrlDifferent = !url.Equals(currentUrl, StringComparison.OrdinalIgnoreCase);

                    Logger.LogTrace($"Is URL Different? '{(isUrlDifferent ? "Yes" : "No")}'");

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

        private IEnumerable<string> GetBikeDetailUrlsFromPage(int pageNumber)
        {
            var bikeDetailsPagesToScrape = new List<string>();

            var currentPageNumber = GetCurrentPageNumberFromUrl();
            if (pageNumber != currentPageNumber)
                throw new ArgumentException($"'{pageNumber}' is not equal to the page number defined in the URL ('{currentPageNumber}')", nameof(pageNumber));

            Logger.LogDebug($"Getting bike detail page URLs from page #{pageNumber}");

            var keywordBlacklist = new List<string>
            {
                "Frameset",
                "TT Disc Module"
            };

            foreach (var product in Products)
            {
                var url = product.FindElement(By.CssSelector("a")).GetAttribute("href").Trim();
                var name = product.FindElement(By.CssSelector("h3")).Text.Trim();

                var matches = new List<string>();

                foreach (var keyword in keywordBlacklist)
                {
                    if (name.Contains(keyword))
                    {
                        Logger.LogWarning($"'{name}' contains a blacklisted keyword ('{keyword}')");

                        matches.Add(keyword);
                    }
                }

                if (matches.Any())
                {
                    var stringBuilder = new StringBuilder();
                    
                    stringBuilder.AppendLine($"'{name}' contains a blacklisted keywords:");

                    foreach (var match in matches)
                    {
                        stringBuilder.AppendLine($"'{match}'");
                    }

                    stringBuilder.AppendLine($"Skipping '{name}'");

                    var message = stringBuilder.ToString();
                    
                    Logger.LogWarning(message);
                }

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
            var implicitWait = WebDriver.Manage().Timeouts().ImplicitWait;
            
            WebDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);

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
            finally
            {
                WebDriver.Manage().Timeouts().ImplicitWait = implicitWait;
            }
        }
    }
}
