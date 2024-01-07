using Data.Pages;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;

namespace Data.Husqvarna.Pages.Shared
{
    public abstract class Page<T> : BasePage<T> where T : Page<T>
    {
        protected Page(ILogger logger, IWebDriver webDriver) : base(logger, webDriver)
        {
            MainMenuElement = new(logger, webDriver);
        }

        public MainMenuElement MainMenuElement { get; set; }
    }
}
