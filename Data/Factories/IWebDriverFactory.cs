using OpenQA.Selenium;

namespace Data.Factories
{
    public interface IWebDriverFactory
    {
        IWebDriver GetWebDriver(TimeSpan implicitWait, bool headless = false);
        IWebDriver GetWebDriver<TWebDriver>(TimeSpan implicitWait, bool headless = false) where TWebDriver : IWebDriver;
    }
}