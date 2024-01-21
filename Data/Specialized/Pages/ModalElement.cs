using Data.Pages;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace Data.Specialized.Pages
{
    public class ModalElement : BaseElement<ModalElement>
    {
        public ModalElement(ILogger logger, IWebDriver webDriver) : base(logger, webDriver)
        {
        }

        public string GetDescription()
        {
            var cssSelectors = new List<string>()
            {
                ".paragraph-wrapper",
                ".modal__content-wrapper",
                ".sc-45035464-0.kmmKVJ"
            };

            string? description = null;
            var exceptions = new List<Exception>();

            foreach (var cssSelector in cssSelectors)
            {
                try
                {
                    description = GetDescriptionFrom(cssSelector);
                }
                catch (Exception e)
                {
                    Logger.LogError(e, $"Failed to get description from '{cssSelector}'");

                    exceptions.Add(e);
                }
            }

            // Close the modal by clicking the "X" button
            //const string closeButtonCssSelector = ".sc-56219169-0.eRyXhD";
            const string closeButtonCssSelector = ".jUEINs";
            WebDriver.FindElement(By.CssSelector(closeButtonCssSelector)).Click();

            if (description is null)
                throw new AggregateException("Failed to get description", exceptions);

            return description;
        }

        private string GetDescriptionFrom(string cssSelector)
        {
            Logger.LogTrace($"Attempting to find the description element ('{cssSelector}')");

            var descriptionLocator = By.CssSelector(cssSelector);

            new WebDriverWait(WebDriver, TimeSpan.FromSeconds(5))
                .Until(ExpectedConditions.ElementIsVisible(descriptionLocator));

            var description = WebDriver.FindElement(descriptionLocator).GetAttribute("innerHTML");

            return description;
        }
    }
}