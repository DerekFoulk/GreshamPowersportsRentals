using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using SeleniumExtras.PageObjects;

namespace Data.Pages
{
    public abstract class BasePage<T> where T : BasePage<T>
    {
        protected BasePage(ILogger logger, IWebDriver webDriver)
        {
            Logger = logger;
            WebDriver = webDriver;

            UpdatePageObject();
        }

        protected BasePage(ILogger logger, IWebDriver webDriver, string url)
        {
            Logger = logger;
            WebDriver = webDriver;
            Url = url;

            GoToPage();

            UpdatePageObject();
        }

        protected ILogger Logger { get; }

        protected IWebDriver WebDriver { get; }

        protected string Url { get; set; }

        protected void GoToPage()
        {
            Logger.LogTrace($"Navigating to '{Url}'");

            WebDriver.Navigate().GoToUrl(Url);
        }

        protected void UpdatePageObject()
        {
            Logger.LogTrace("Reinitializing elements for page object");
            PageFactory.InitElements(WebDriver, this);
        }
    }
}
