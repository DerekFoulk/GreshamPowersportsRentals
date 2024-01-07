using Data.Husqvarna.Pages.Shared;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;

namespace Data.Husqvarna.Pages
{
    public class HomePage : Page<HomePage>
    {
        public HomePage(ILogger logger, IWebDriver webDriver) : base(logger, webDriver)
        {
            GoToPage();
        }

        public void GoToPage()
        {
            Logger.LogTrace("Navigating to the home page");

            WebDriver.Navigate().GoToUrl("https://www.husqvarna-bicycles.com/en-us.html");
        }
    }
}
