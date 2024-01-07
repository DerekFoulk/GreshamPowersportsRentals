using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using SeleniumExtras.PageObjects;

namespace Data.Pages
{
    public abstract class BaseElement<T> where T : BaseElement<T>
    {
        protected BaseElement(ILogger logger, IWebDriver webDriver)
        {
            Logger = logger;
            WebDriver = webDriver;

            Logger.LogTrace("Reinitializing elements for element object");
            PageFactory.InitElements(WebDriver, this);
        }

        protected ILogger Logger { get; }

        protected IWebDriver WebDriver { get; }
    }
}
