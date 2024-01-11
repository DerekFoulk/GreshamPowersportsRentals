using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;
using Data.Extensions;
using Data.Pages;
using Data.Specialized.Exceptions;
using Data.Specialized.Models;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.PageObjects;
using SeleniumExtras.WaitHelpers;

namespace Data.Specialized.Pages
{
    public class BikeDetailsPage : BasePage<BikeDetailsPage>
    {
        public BikeDetailsPage(ILogger logger, IWebDriver webDriver) : base(logger, webDriver)
        {
        }

        [FindsBy(How = How.CssSelector, Using = "nav[aria-label=\"breadcrumb\"]")]
        public IWebElement? Breadcrumbs { get; set; }

        [FindsBy(How = How.CssSelector, Using = "div[data-component=\"sidebar\"]")]
        public IWebElement? Sidebar { get; set; }

        [FindsBy(How = How.CssSelector, Using = "div[data-component=\"gallery-wrapper\"]")]
        public IWebElement? Gallery { get; set; }

        [FindsBy(How = How.CssSelector, Using = "div[data-component=\"product-detail-header\"]")]
        public IWebElement? ProductDetailHeader { get; set; }

        [FindsBy(How = How.CssSelector, Using = "div.sc-7881ce43-5.kNHxFW h5")]
        public IList<IWebElement>? Prices { get; set; }

        [FindsBy(How = How.CssSelector, Using = "div[data-component=\"color-selection\"] button")]
        public IList<IWebElement>? ColorButtons { get; set; }

        [FindsBy(How = How.CssSelector, Using = "p.sc-e4145e4c-10.fhNxpW")]
        public IWebElement? ColorLabel { get; set; }

        [FindsBy(How = How.CssSelector, Using = "div[data-component=\"size-selection\"] button")]
        public IList<IWebElement>? SizeButtons { get; set; }

        [FindsBy(How = How.CssSelector, Using = "button.sc-d636bba-0.sc-d636bba-3.qDQJb.bzPHbY")]
        public IWebElement? ReadMoreButton { get; set; }

        [FindsBy(How = How.Id, Using = "modal-root")]
        public IWebElement? Modal { get; set; }

        [FindsBy(How = How.CssSelector, Using = "div[data-component=\"technical-specifications-accordion\"]")]
        public IWebElement? TechnicalSpecificationsAccordion { get; set; }

        public Model GetBikeDetails()
        {
            Logger.LogTrace($"Getting bike details from page ('{WebDriver.Url}')");

            var name = ProductDetailHeader.FindElement(By.TagName("h1")).Text.Trim();

            Logger.LogTrace($"Name: {name}");

            var description = GetDescription();

            Logger.LogTrace($"Description: {description}");

            var videos = GetVideos()?.ToList();

            Logger.LogTrace($"Videos: {videos?.Count ?? 0}");

            var modelConfigurations = GetModelConfigurations(name);

            TechnicalSpecifications technicalSpecifications = null;
            List<ManualDownload> manualDownloads = null;

            var model = new Model(name, description, technicalSpecifications, manualDownloads, modelConfigurations, videos);
            
            return model;
        }

        private IEnumerable<string>? GetVideos()
        {
            IEnumerable<string>? videos = null;
            
            try
            {
                Logger.LogTrace("Scraping YouTube video links");

                var videoLinks = WebDriver.FindElements(By.CssSelector("a[href*=\"youtube\"]"));

                Logger.LogTrace("Getting 'href' attributes from all video links");

                videos = videoLinks.Select(x => x.GetAttribute("href"));
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Failed to scrape videos");

                var fileName = $"{DateTime.Now:yyyyMMddHHmmss}_{GetType().FullName}_{MethodBase.GetCurrentMethod()?.Name}_{e.GetType().FullName}";

                try
                {
                    Logger.LogDebug($"Capturing HTML and screenshot ('{fileName}')");

                    WebDriver.CaptureHtmlAndScreenshot(fileName);
                }
                catch (Exception exception)
                {
                    Logger.LogError(exception, $"Failed to save HTML and screenshot ('{fileName}')");
                }
            }

            return videos;
        }

        private IEnumerable<ModelConfiguration> GetModelConfigurations(string name)
        {
            Logger.LogTrace($"Getting model configurations for '{name}'");

            WaitForPageToLoad();

            var modelConfigurations = new List<ModelConfiguration>();

            var geometryTable = GetGeometryTable();

            foreach (var sizeButton in SizeButtons)
            {
                Logger.LogTrace($"Clicking '{sizeButton.Text}' button");

                Logger.LogTrace($"Scrolling to '{nameof(sizeButton)}'");
                
                WebDriver.ScrollToElement(sizeButton);

                sizeButton.Click();

                var size = sizeButton.Text;

                Logger.LogTrace($"Size: {size}");

                var geometry = GetGeometryFromGeometryTable(size, geometryTable);

                foreach (var colorButton in ColorButtons)
                {
                    Logger.LogTrace($"Scrolling to '{nameof(colorButton)}'");
                    
                    WebDriver.ScrollToElement(colorButton);
                    
                    colorButton.Click();

                    var color = WebDriver
                        .FindElement(By.CssSelector("[data-component=\"color-selection\"] p:last-child")).Text;

                    Logger.LogTrace($"Clicked '{color}' button");

                    Logger.LogTrace($"Color: {color}");

                    Logger.LogTrace($"Getting configuration for '{color}'/'{size}'");

                    var partNumber = GetPartNumber();

                    var pricing = GetPricing();

                    // TODO: Move this to a color only loop as we don't need images for each color/size combination (I think)
                    var images = GetImages();

                    var modelConfiguration = new ModelConfiguration(partNumber, pricing, color, images, size, geometry);

                    modelConfigurations.Add(modelConfiguration);
                }
            }
            
            return modelConfigurations;
        }

        private Geometry GetGeometryFromGeometryTable(string size, GeometryTable geometryTable)
        {
            Logger.LogTrace($"Getting geometry from the geometry table object for '{size}'");

            var geometryTableRows = geometryTable.GeometryTableRows;

            var dimensions = new Dictionary<Dimension, string>();

            foreach (var geometryTableRow in geometryTableRows)
            {
                var dimension = new Dimension(geometryTableRow.DimensionName, geometryTableRow.ImageUrl);

                Logger.LogTrace($"Dimension Name: {dimension.Name}");
                Logger.LogTrace($"Dimension Image URL: {dimension.ImageUrl}");

                var sizeAndValuePairs =
                    geometryTableRow.SizeAndValuePairs.Where(svp =>
                        svp.Key.Equals(size, StringComparison.OrdinalIgnoreCase));

                foreach (var (key, value) in sizeAndValuePairs)
                {
                    Logger.LogTrace($"Key (Size): {key}");
                    Logger.LogTrace($"Value: {value}");

                    dimensions.Add(dimension, value);
                }
            }

            var geometry = new Geometry(dimensions);

            return geometry;
        }

        private GeometryTable GetGeometryTable()
        {
            Logger.LogTrace("Getting geometry for each size");

            Logger.LogTrace("Expanding the 'Geometry' accordion item");

            // Expand the "Geometry" accordion item
            TechnicalSpecificationsAccordion.FindElements(By.TagName("h3")).Single(x =>
                x.Text.Trim().Equals("Geometry", StringComparison.OrdinalIgnoreCase)).Click();

            Logger.LogTrace("Getting 'th' text values to determine which column to read");

            var geometryTableHead = TechnicalSpecificationsAccordion.FindElement(By.TagName("thead"));
            var columnHeaderValues = geometryTableHead
                .FindElements(By.TagName("th")).Select(x => x.Text.Trim()).ToList();

            var geometryTableBody = TechnicalSpecificationsAccordion.FindElement(By.TagName("tbody"));
            var rows = geometryTableBody.FindElements(By.TagName("tr"));

            Logger.LogTrace("Scraping dimensions from table rows");

            var geometryTableRows = new List<GeometryTableRow>();

            foreach (var row in rows)
            {
                var rowIndex = rows.IndexOf(row);

                try
                {
                    var geometryTableRow = GetGeometryTableRow(row, rowIndex, columnHeaderValues);
                    geometryTableRows.Add(geometryTableRow);
                }
                catch (Exception e)
                {
                    Logger.LogError(e, "Failed to get dimensions from row");
                }
            }

            var geometryTable = new GeometryTable(geometryTableRows);
            
            return geometryTable;
        }

        // TODO: Add scraping of the image URL for the dimension visual aid
        private GeometryTableRow GetGeometryTableRow(IWebElement row, int rowIndex,
            List<string> columnHeaderValues)
        {
            ArgumentNullException.ThrowIfNull(row);

            Logger.LogTrace($"Getting dimension from row #{rowIndex}");

            Logger.LogTrace($"Scrolling to '{nameof(row)}' (#{rowIndex})");

            WebDriver.ScrollToElement(row);

            var columns = row.FindElements(By.CssSelector("td"));

            if (columns is null)
                throw new NullReferenceException($"'{nameof(columns)}' cannot be null");

            // Throw an exception if any td elements have a colspan or rowspan set as it would cause misinterpretation of values
            ThrowIfTableLayoutIsUnexpected(columns);

            Logger.LogTrace($"Getting dimension from row #{rowIndex}");

            var dimensionName = columns.First().Text.Trim();
            Logger.LogTrace($"Dimension Name: {dimensionName}");

            var valueColumns = columns.Skip(1);

            var values = new Dictionary<string, string>();

            foreach (var valueColumn in valueColumns)
            {
                var columnIndex = columns.IndexOf(valueColumn);
                var size = columnHeaderValues.ElementAt(columnIndex);
                
                var dimensionValue = valueColumn.Text.Trim();
                Logger.LogTrace($"Dimension Value: {dimensionValue}");

                values.Add(size, dimensionValue);
            }

            var imageUrl = WebDriver.FindElement(By.CssSelector("img[alt=\"Geometry\"]")).GetAttribute("src");

            var geometryTableRow = new GeometryTableRow(dimensionName, values, imageUrl);

            return geometryTableRow;
        }

        private void WaitForPageToLoad()
        {
            var stopwatch = Stopwatch.StartNew();

            Logger.LogTrace("Waiting for page to load...");

            new WebDriverWait(WebDriver, TimeSpan.FromSeconds(5))
                .Until(ExpectedConditions.ElementExists(By.CssSelector(".swiper-wrapper")));

            stopwatch.Stop();

            Logger.LogTrace($"Page loaded after '{stopwatch.Elapsed}'");
        }

        private void GetDimensionFromRow(IWebElement row, int focusedColumnIndex, Dictionary<string, string> dimensions,
            int rowIndex, string size)
        {
            ArgumentNullException.ThrowIfNull(row);

            Logger.LogTrace($"Getting dimension from row #{rowIndex}");

            Logger.LogTrace($"Scrolling to '{nameof(row)}' (#{rowIndex})");

            WebDriver.ScrollToElement(row);

            var columns = row.FindElements(By.TagName("td"));

            if (columns is null)
                throw new NullReferenceException($"'{nameof(columns)}' cannot be null");

            if (columns.Count < (focusedColumnIndex + 1))
                throw new ArgumentOutOfRangeException(nameof(focusedColumnIndex), $"Focused column index ({focusedColumnIndex}) exceeds column count ({columns.Count}) in row #{rowIndex}");

            // Throw an exception if any td elements have a colspan or rowspan set as it would cause misinterpretation of values
            ThrowIfTableLayoutIsUnexpected(columns);

            Logger.LogTrace($"Getting dimension from row #{rowIndex}");

            var key = columns.First().Text.Trim();
            Logger.LogTrace($"Name: {key}");

            var value = columns.ElementAt(focusedColumnIndex).Text.Trim();
            Logger.LogTrace($"Value: {value}");

            Logger.LogTrace($"Attempting to add dimension to '{nameof(dimensions)}'");

            if (!dimensions.TryAdd(key, value))
            {
                var stringBuilder = new StringBuilder();

                stringBuilder.AppendLine($"Failed to add the '{key}' dimension:");
                stringBuilder.AppendLine($"{nameof(key)}: {key}");
                stringBuilder.AppendLine($"{nameof(value)}: {value}");
                stringBuilder.AppendLine($"{nameof(row)}: {rowIndex}");
                stringBuilder.AppendLine($"{nameof(focusedColumnIndex)}: {focusedColumnIndex}");
                stringBuilder.AppendLine($"{nameof(size)}: {size}");
                stringBuilder.AppendLine($"{nameof(WebDriver)}.{nameof(WebDriver.Url)}: {WebDriver.Url}");

                var message = stringBuilder.ToString();

                throw new Exception(message);
            }

            Logger.LogTrace("Dimension was added successfully");
        }

        private void ThrowIfTableLayoutIsUnexpected(ReadOnlyCollection<IWebElement> columns)
        {
            Logger.LogTrace("Validating table layout");

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

                    var isInvalidValue = false;

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

            Logger.LogTrace("Table structure is valid");
        }

        private IEnumerable<string> GetImages()
        {
            Logger.LogTrace($"Getting images from '{nameof(Gallery)}'");

            var images = new List<string>();

            var imgElements = Gallery.FindElements(By.TagName("img"));

            Logger.LogTrace($"Found {imgElements.Count} 'img' elements in the '{nameof(Gallery)}' element");

            foreach (var imgElement in imgElements)
            {
                var src = imgElement.GetAttribute("src").Trim();

                Logger.LogTrace($"Adding '{src}' to images");

                images.Add(src);
            }

            Logger.LogTrace($"Scraped {images.Count} images from '{nameof(Gallery)}'");

            return images;
        }

        private Pricing GetPricing()
        {
            Logger.LogTrace($"Getting pricing from '{nameof(Prices)}'");

            var amounts = new List<decimal>();

            foreach (var webElement in Prices)
            {
                var amount = decimal.Parse(webElement.Text, NumberStyles.Currency);

                Logger.LogTrace($"Adding price of '{amount:C}'");

                amounts.Add(amount);
            }

            var msrp = amounts.Max();

            Logger.LogTrace($"MSRP: '{msrp:C}'");

            var price = amounts.Min();
            
            Logger.LogTrace($"Price: '{price:C}'");

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

            var modalLocator = this.GetByFrom(nameof(Modal));

            new WebDriverWait(WebDriver, TimeSpan.FromSeconds(5))
                .Until(ExpectedConditions.ElementIsVisible(modalLocator));

            ////Logger.LogTrace("Reinitializing elements for page object");
            ////PageFactory.InitElements(WebDriver, this);

            ////const string cssSelector = "div.sc-45035464-0.kmmKVJ";
            //const string cssSelector = ".paragraph-wrapper";

            //Logger.LogTrace($"Attempting to find the description element ('{cssSelector}')");

            //var descriptionLocator = By.CssSelector(cssSelector);

            //try
            //{
            //    Logger.LogTrace($"Waiting for the description element ('{cssSelector}') to become visible");

            //    new WebDriverWait(WebDriver, TimeSpan.FromSeconds(3))
            //        .Until(ExpectedConditions.ElementIsVisible(descriptionLocator));
            //}
            //catch (WebDriverTimeoutException e)
            //{
            //    Logger.LogError(e, $"Could not see the '{cssSelector}' element");

            //    var explodedCssSelectors = cssSelector.Split('.').ToList();
            //    var cssSelectors = explodedCssSelectors.Where(x => explodedCssSelectors.IndexOf(x) != 0).ToList();

            //    Logger.LogTrace($"Checking page source for presence of the '{cssSelector}' element");

            //    ThrowIfPageSourceContainsElementWithAnySelectors(cssSelectors);

            //    throw;
            //}

            //var description = Modal.FindElement(descriptionLocator).GetAttribute("innerHTML");

            var description = Modal.Text;

            // Close the modal by clicking the "X" button
            //const string closeButtonCssSelector = ".sc-56219169-0.eRyXhD";
            const string closeButtonCssSelector = ".jUEINs";
            Modal.FindElement(By.CssSelector(closeButtonCssSelector)).Click();

            return description;
        }
    }
}
