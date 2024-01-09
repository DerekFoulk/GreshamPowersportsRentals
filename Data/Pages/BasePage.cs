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

        protected ILogger Logger { get; }

        protected IWebDriver WebDriver { get; }

        protected void UpdatePageObject()
        {
            Logger.LogTrace("Reinitializing elements for page object");
            PageFactory.InitElements(WebDriver, this);
        }
    }
}
