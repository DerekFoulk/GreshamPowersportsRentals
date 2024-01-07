using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Text;
using Data.Husqvarna.Pages.Shared;
using Data.Specialized.Exceptions;
using Data.Specialized.Models;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.PageObjects;
using SeleniumExtras.WaitHelpers;

namespace Data.Specialized.Pages
{
    public class BikeDetailsPage : Page<BikeDetailsPage>
    {
        public BikeDetailsPage(ILogger logger, IWebDriver webDriver) : base(logger, webDriver)
        {
        }

        [FindsBy(How = How.CssSelector, Using = "nav[aria-label=\"breadcrumb\"]")]
        public IWebElement Breadcrumbs { get; set; }

        [FindsBy(How = How.CssSelector, Using = "div[data-component=\"sidebar\"]")]
        public IWebElement Sidebar { get; set; }

        [FindsBy(How = How.CssSelector, Using = "div[data-component=\"gallery-wrapper\"]")]
        public IWebElement Gallery { get; set; }

        [FindsBy(How = How.CssSelector, Using = "div[data-component=\"product-detail-header\"]")]
        public IWebElement ProductDetailHeader { get; set; }

        [FindsBy(How = How.CssSelector, Using = "div.sc-7881ce43-5.kNHxFW h5")]
        public IList<IWebElement> Prices { get; set; }

        [FindsBy(How = How.CssSelector, Using = "div[data-component=\"color-selection\"] button")]
        public IList<IWebElement> ColorButtons { get; set; }

        [FindsBy(How = How.CssSelector, Using = "p.sc-e4145e4c-10.fhNxpW")]
        public IWebElement ColorLabel { get; set; }

        [FindsBy(How = How.CssSelector, Using = "div[data-component=\"size-selection\"] button")]
        public IList<IWebElement> SizeButtons { get; set; }

        [FindsBy(How = How.CssSelector, Using = "button.sc-d636bba-0.sc-d636bba-3.qDQJb.bzPHbY")]
        public IWebElement ReadMoreButton { get; set; }

        [FindsBy(How = How.Id, Using = "modal-root")]
        public IWebElement Modal { get; set; }

        [FindsBy(How = How.CssSelector, Using = "div[data-component=\"technical-specifications-accordion\"]")]
        public IWebElement TechnicalSpecificationsAccordion { get; set; }

        public Model GetBikeDetails()
        {
            Logger.LogTrace($"Getting bike details from page ('{WebDriver.Url}')");

            var name = ProductDetailHeader.FindElement(By.TagName("h1")).Text.Trim();

            Logger.LogTrace($"Name: {name}");

            var description = GetDescription();

            Logger.LogTrace($"Description: {description}");

            var modelConfigurations = GetModelConfigurations(name);

            TechnicalSpecifications technicalSpecifications = null;
            List<ManualDownload> manualDownloads = null;

            var model = new Model(name, description, technicalSpecifications, manualDownloads, modelConfigurations);
            
            return model;
        }

        private IEnumerable<ModelConfiguration> GetModelConfigurations(string name)
        {
            Logger.LogTrace($"Getting model configurations for '{name}'");

            var modelConfigurations = new List<ModelConfiguration>();

            foreach (var sizeButton in SizeButtons)
            {
                Logger.LogTrace($"Clicking '{sizeButton.Text}' button");

                sizeButton.Click();

                var size = Enum.Parse<SpecializedBikeSize>(sizeButton.Text);

                Logger.LogTrace($"Size: {size}");

                foreach (var colorButton in ColorButtons)
                {
                    Logger.LogTrace($"Clicking '{colorButton.Text}' button");

                    colorButton.Click();

                    var color = WebDriver.FindElement(By.CssSelector("p.sc-e4145e4c-10.fhNxpW")).Text.Trim();

                    Logger.LogTrace($"Color: {color}");

                    Logger.LogTrace($"Getting configuration for '{color}'/'{size}'");

                    var partNumber = GetPartNumber();

                    var pricing = GetPricing();

                    var images = GetImages();

                    var geometry = GetGeometry(size);

                    var modelConfiguration = new ModelConfiguration(partNumber, pricing, color, images, size, geometry);

                    modelConfigurations.Add(modelConfiguration);
                }
            }

            return modelConfigurations;
        }

        private Geometry GetGeometry(SpecializedBikeSize size)
        {
            Logger.LogTrace($"Getting geometry for '{size}'");

            Logger.LogTrace("Expanding the 'Geometry accordion item");

            // Expand the "Geometry" accordion item
            TechnicalSpecificationsAccordion.FindElements(By.TagName("h3")).Single(x =>
                x.Text.Trim().Equals("Geometry", StringComparison.OrdinalIgnoreCase)).Click();

            var geometryTableHead = TechnicalSpecificationsAccordion.FindElement(By.TagName("table thead"));
            var columnHeaderValues = geometryTableHead
                .FindElements(By.TagName("th")).Select(x => x.Text.Trim()).ToList();
            var focusedColumn = columnHeaderValues.IndexOf(size.ToString());
            var geometryTableBody = TechnicalSpecificationsAccordion.FindElement(By.TagName("table tbody"));
            var rows = geometryTableBody.FindElements(By.TagName("tr"));

            var dimensions = new Dictionary<string, string>();

            foreach (var row in rows)
            {
                var rowIndex = rows.IndexOf(row);

                try
                {
                    GetDimensionFromRow(row, focusedColumn, dimensions, rowIndex, size);
                }
                catch (Exception e)
                {
                    Logger.LogError(e, "Failed to get dimension from row");
                    
                    throw;
                }
            }

            var geometry = new Geometry(size, dimensions);

            return geometry;
        }

        private void GetDimensionFromRow(IWebElement row, int focusedColumn, Dictionary<string, string> dimensions,
            int rowIndex, SpecializedBikeSize size)
        {
            var columns = row.FindElements(By.TagName("td"));

            // Throw an exception if any td elements have a colspan or rowspan set as it would cause misinterpretation of values
            ThrowIfTableLayoutIsUnexpected(columns);

            var key = columns.First().Text.Trim();
            var value = columns.ElementAt(focusedColumn).Text.Trim();

            if (!dimensions.TryAdd(key, value))
            {
                var stringBuilder = new StringBuilder();

                stringBuilder.AppendLine($"Failed to add the '{key}' dimension:");
                stringBuilder.AppendLine($"{nameof(key)}: {key}");
                stringBuilder.AppendLine($"{nameof(value)}: {value}");
                stringBuilder.AppendLine($"{nameof(row)}: {rowIndex}");
                stringBuilder.AppendLine($"{nameof(focusedColumn)}: {focusedColumn}");
                stringBuilder.AppendLine($"{nameof(size)}: {size}");
                stringBuilder.AppendLine($"{nameof(WebDriver)}.{nameof(WebDriver.Url)}: {WebDriver.Url}");

                var message = stringBuilder.ToString();

                throw new Exception(message);
            }
        }

        private void ThrowIfTableLayoutIsUnexpected(ReadOnlyCollection<IWebElement> columns)
        {
            var attributes = new List<string>
            {
                "colspan",
                "rowspan"
            };

            var invalidStates = new List<Func<IWebElement, bool>>();

            foreach (var attribute in attributes)
            {
                Func<IWebElement, bool> func = column =>
                {
                    var attributeValue = column.GetAttribute(attribute);
                    var isSet = !string.IsNullOrWhiteSpace(attributeValue);

                    bool isInvalidValue = false;

                    if (isSet)
                    {
                        try
                        {
                            isInvalidValue = int.Parse(attributeValue) > 1;
                        }
                        catch (Exception e)
                        {
                            Logger.LogWarning(e, $"Failed to parse int from '{attribute}' value");
                        }
                    }

                    return isInvalidValue;
                };

                invalidStates.Add(func);
            }

            var isTableStructureInvalid = false;

            foreach (var invalidState in invalidStates)
            {
                var invalid = columns.Any(invalidState);

                if (invalid)
                    isTableStructureInvalid = true;
            }

            if (isTableStructureInvalid)
                throw new UnexpectedTableLayoutException("Unexpected table structure encountered");
        }

        private IEnumerable<string> GetImages()
        {
            var images = new List<string>();

            var imgElements = Gallery.FindElements(By.TagName("img"));

            foreach (var imgElement in imgElements)
            {
                var src = imgElement.GetAttribute("src").Trim();

                images.Add(src);
            }

            return images;
        }

        private Pricing GetPricing()
        {
            var amounts = new List<decimal>();

            foreach (var webElement in Prices)
            {
                var amount = decimal.Parse(webElement.Text);

                amounts.Add(amount);
            }

            var msrp = amounts.Max();
            var price = amounts.Min();
            var pricing = new Pricing(msrp, price);

            return pricing;
        }

        private string GetPartNumber()
        {
            Logger.LogTrace("Getting part number");

            var text = ProductDetailHeader.FindElement(By.CssSelector("p span")).Text;
            var split = text.Split(':', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var partNumber = split.Last().Trim();

            Logger.LogTrace($"Parsed '{partNumber}' from '{text}'");

            return partNumber;
        }

        private string GetDescription()
        {
            Logger.LogTrace("Getting description");

            Logger.LogTrace("Clicking 'Read More' button (to open the description modal)");

            // Open the modal by clicking the "Read More" button
            ReadMoreButton.Click();

            Logger.LogTrace("Reinitializing elements for page object");
            PageFactory.InitElements(WebDriver, this);

            const string cssSelector = "div.sc-45035464-0.kmmKVJ";

            Logger.LogTrace($"Attempting to find the description element ('{cssSelector}')");

            var descriptionBy = By.CssSelector(cssSelector);

            try
            {
                Logger.LogTrace($"Waiting for the description element ('{cssSelector}') to become visible");

                new WebDriverWait(WebDriver, TimeSpan.FromSeconds(10))
                    .Until(ExpectedConditions.ElementIsVisible(descriptionBy));
            }
            catch (WebDriverTimeoutException e)
            {
                Logger.LogError(e, $"Could not see the '{cssSelector}' element");

                var explodedCssSelectors = cssSelector.Split('.').ToList();
                var cssSelectors = explodedCssSelectors.Where(x => explodedCssSelectors.IndexOf(x) != 0).ToList();

                Logger.LogTrace($"Checking page source for presence of the '{cssSelector}' element");

                ThrowIfPageSourceContainsElementWithAnySelectors(cssSelectors);

                throw;
            }

            var description = Modal.FindElement(descriptionBy).GetAttribute("innerHTML");

            // Close the modal by clicking the "X" button
            Modal.FindElement(By.CssSelector(".sc-56219169-0.eRyXhD")).Click();

            return description;
        }

        private void ThrowIfPageSourceContainsElementWithAnySelectors(List<string> cssSelectors)
        {
            var html = WebDriver.PageSource;

            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine(
                "Checking page source for the presence of elements with a selector matching any of the following:");
            foreach (var cssSelector in cssSelectors)
            {
                stringBuilder.AppendLine($"    - \"{cssSelector}\"");
            }

            var message = stringBuilder.ToString();
            Logger.LogTrace(message);

            foreach (var cssSelector in cssSelectors)
            {
                if (html.Contains(cssSelector))
                    throw new Exception($"Source code contains element with the '{cssSelector}' selector");
            }
        }
    }
}
