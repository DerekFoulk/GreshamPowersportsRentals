using System.Diagnostics;
using System.Text;
using Data.Pages;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.PageObjects;

namespace Data.Specialized.Pages
{
    public class BikesPage : BasePage<BikesPage>
    {
        public BikesPage(ILogger logger, IWebDriver webDriver) : base(logger, webDriver)
        {
        }

        [FindsBy(How = How.CssSelector, Using = "[data-component=\"product-tile\"]")]
        public IList<IWebElement> Products { get; set; }

        public void GoToPage(int pageNumber = 1)
        {
            if (pageNumber <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageNumber), $"'{nameof(pageNumber)}' must be greater than zero");

            var url = $"https://www.specialized.com/us/en/c/bikes?group=Bikes&group=E-Bikes&page={pageNumber}";
            
            Logger.LogTrace($"Navigating to the 'Bikes' page #{pageNumber} ('{url}')");

            WebDriver.Navigate().GoToUrl(url);
        }

        public IEnumerable<string> GetBikeDetailUrlsAcrossPages(int? maxPage = null, int? minPage = null)
        {
            Logger.LogDebug("Getting bike detail page URLs");

            var bikeDetailsPagesToScrape = new List<string>();

            var currentPageNumber = GetCurrentPageNumberFromUrl();

            Logger.LogTrace($"Getting bike detail page URLs from page #{currentPageNumber}");

            var stopwatch = Stopwatch.StartNew();

            while (!IsLastPage())
            {
                currentPageNumber = GetCurrentPageNumberFromUrl();

                if (stopwatch.Elapsed >= TimeSpan.FromMinutes(3))
                {
                    stopwatch.Stop();

                    throw new TimeoutException($"Getting bike detail page URLs timed out after '{stopwatch.Elapsed}'");
                }

                if (IsPageWithinRange(currentPageNumber, maxPage, minPage))
                {
                    var urls = GetBikeDetailUrlsFromPage(currentPageNumber);

                    bikeDetailsPagesToScrape.AddRange(urls);
                }

                var nextPage = currentPageNumber + 1;

                if (nextPage > maxPage)
                    break;

                if (minPage is not null)
                    if (nextPage < minPage)
                        nextPage = (int)minPage;

                GoToPage(nextPage);
            }

            return bikeDetailsPagesToScrape;
        }

        private bool IsPageWithinRange(int pageNumber, int? maxPage = null, int? minPage = null)
        {
            if (maxPage is null && minPage is null)
                return true;

            if (maxPage is not null && minPage is not null)
            {
                if (maxPage < minPage)
                    throw new ArgumentException($"'{nameof(maxPage)}' cannot be less than '{nameof(minPage)}'");

                return pageNumber >= minPage && pageNumber <= maxPage;
            }

            if (minPage is not null)
                return pageNumber >= minPage;

            if (maxPage is not null)
                return pageNumber <= maxPage;

            return false;
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
                else
                {
                    Logger.LogTrace($"Adding '{name}' ({url}) #{pageNumber}");

                    bikeDetailsPagesToScrape.Add(url);
                }
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
            var newImplicitWait = TimeSpan.FromSeconds(2);
            
            Logger.LogTrace($"Temporarily setting implicit wait timeout to '{newImplicitWait}'");
            
            var implicitWait = WebDriver.Manage().Timeouts().ImplicitWait;
            
            WebDriver.Manage().Timeouts().ImplicitWait = newImplicitWait;

            Logger.LogTrace("Checking if the current page is the last page");

            try
            {
                Logger.LogTrace("Looking for 'next page' button");

                WebDriver.FindElement(By.CssSelector("button.sc-aecdb1f6-0.hHInlF"));

                Logger.LogTrace("Found the 'next page' button");

                Logger.LogTrace("Is not the last page");

                return false;
            }
            catch (NoSuchElementException)
            {
                Logger.LogTrace("Is the last page");

                return true;
            }
            finally
            {
                Logger.LogTrace($"Resetting implicit wait timeout to '{implicitWait}'");

                WebDriver.Manage().Timeouts().ImplicitWait = implicitWait;
            }
        }
    }
}
