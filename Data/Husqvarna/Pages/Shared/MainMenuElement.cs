using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using SeleniumExtras.PageObjects;

namespace Data.Husqvarna.Pages.Shared
{
    public class MainMenuElement : Element<MainMenuElement>
    {
        public MainMenuElement(ILogger logger, IWebDriver webDriver) : base(logger, webDriver)
        {
        }

        [FindsBy(How = How.ClassName, Using = "js-menu-toggle")]
        public IWebElement? MenuToggle { get; set; }

        [FindsBy(How = How.CssSelector, Using = ".nav ul.level-1 > li")]
        // ReSharper disable once CollectionNeverUpdated.Global
        public IList<IWebElement>? MenuItems { get; set; }

        public List<IWebElement> GetModelMenuItems()
        {
            Logger.LogTrace("Getting models");

            // Open the main menu
            Logger.LogTrace("Opening the main menu");

            Logger.LogTrace("Clicking the menu toggle");
            MenuToggle?.Click();

            // Open the Models submenu
            Logger.LogTrace("Clicking the 'Models' menu item");
            var modelsMenuItem =
                MenuItems?.First(x => x.Text.Trim().Contains("Models", StringComparison.OrdinalIgnoreCase));
            var modelsHeading = modelsMenuItem?.FindElement(By.CssSelector(".heading"));
            modelsHeading?.Click();

            // Open the Models sub menu
            var modelsSubmenuItems = modelsMenuItem?.FindElements(By.CssSelector("ul.level-2 > li"));

            // Throw an exception if Models submenu items are not found
            if (modelsSubmenuItems == null)
                throw new NullReferenceException($"'{nameof(modelsSubmenuItems)}' cannot be null");

            var allModelMenuItems = new List<IWebElement>();

            // Open each Models submenu item
            foreach (var modelsSubmenuItem in modelsSubmenuItems)
            {
                var heading = modelsSubmenuItem.FindElement(By.CssSelector(".heading"));

                Logger.LogTrace($"Getting models from '{heading.Text.Trim()}'");
                heading.Click();

                // Extract Model name and URL from each nav item
                var modelMenuItems = new List<IWebElement>(modelsSubmenuItem.FindElements(By.CssSelector("ul.level-4 > li a")));

                Logger.LogTrace($"Found '{allModelMenuItems.Count}' models");

                allModelMenuItems.AddRange(modelMenuItems);
            }

            return allModelMenuItems;
        }
    }
}
