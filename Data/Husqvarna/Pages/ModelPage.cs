using System.Globalization;
using Data.Husqvarna.Models;
using Data.Husqvarna.Pages.Shared;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using SeleniumExtras.PageObjects;

namespace Data.Husqvarna.Pages
{
    public class ModelPage : Page<ModelPage>
    {
        private readonly string _url;
        private readonly string _name;

        public ModelPage(ILogger logger, IWebDriver webDriver, string url, string name) : base(logger, webDriver)
        {
            _url = url;
            _name = name;

            GoToPage();
        }

        [FindsBy(How = How.CssSelector, Using = ".c-variant-overview__selector-title span")]
        public IWebElement Title { get; set; }

        [FindsBy(How = How.CssSelector, Using = ".details-item-value span")]
        public IWebElement ItemValue { get; set; }

        [FindsBy(How = How.CssSelector, Using = ".js-bike-image img")]
        public IWebElement BikeStageImage { get; set; }

        [FindsBy(How = How.CssSelector, Using = ".c-variant-overview__content-image img")]
        public IWebElement OverviewContentImage { get; set; }

        [FindsBy(How = How.CssSelector, Using = ".c-variant-overview__heading.highlight span")]
        public IWebElement OverviewHeadingHighlight { get; set; }

        [FindsBy(How = How.CssSelector, Using = ".c-variant-overview__content-description span")]
        public IWebElement OverviewContentDescription { get; set; }

        public void GoToPage()
        {
            Logger.LogTrace($"Navigating to the model page for {_name}");

            WebDriver.Navigate().GoToUrl(_url);
        }

        public HusqvarnaBicycleInfo GetBicycleInfo()
        {
            Logger.LogTrace($"Getting info for {_name} ({_url})");

            var name = Title.Text;

            Logger.LogTrace($"Name: {name}");

            var numberFormatInfo = new NumberFormatInfo
            {
                CurrencySymbol = "USD",
                CurrencyDecimalSeparator = ".",
                CurrencyGroupSeparator = ","
            };
            var msrp = decimal.Parse(ItemValue.Text.Trim(), NumberStyles.Currency, numberFormatInfo);

            Logger.LogTrace($"MSRP: {msrp}");

            var largeImage = BikeStageImage.GetAttribute("src");

            Logger.LogTrace($"Large Image: {largeImage}");

            string? descriptionImage = null;

            try
            {
                descriptionImage = OverviewContentImage.GetAttribute("src");

                Logger.LogTrace($"Description Image: {descriptionImage}");
            }
            catch (NoSuchElementException e)
            {
                Logger.LogWarning(e, "No image element found");
            }
            
            var images = new HusqvarnaImages(largeImage, descriptionImage);

            var descriptionHeading = OverviewHeadingHighlight.Text.Trim();
            
            Logger.LogTrace($"Description Heading: {descriptionHeading}");
            
            var text = OverviewContentDescription.Text.Trim();

            Logger.LogTrace($"Text: {text}");

            var description = new HusqvarnaDescription(descriptionHeading, text);

            var bicycleInfo = new HusqvarnaBicycleInfo(name, msrp, images, description, default);

            return bicycleInfo;
        }
    }
}
