using System.Diagnostics;
using System.Globalization;
using System.Text;
using Data.Extensions;
using Data.Pages;
using Data.Specialized.Entities;
using Data.Specialized.Exceptions;
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

        [FindsBy(How = How.CssSelector, Using = "nav[aria-label=\"breadcrumb\"] li")]
        public IList<IWebElement>? Breadcrumbs { get; set; }

        [FindsBy(How = How.CssSelector, Using = "div[data-component=\"sidebar\"]")]
        public IWebElement? Sidebar { get; set; }

        [FindsBy(How = How.CssSelector, Using = "div[data-component=\"gallery-wrapper\"]")]
        public IWebElement? Gallery { get; set; }

        [FindsBy(How = How.CssSelector, Using = "div[data-component=\"product-detail-header\"]")]
        public IWebElement? ProductDetailHeader { get; set; }

        [FindsBy(How = How.CssSelector, Using = "div.sc-7881ce43-5.kNHxFW h5")]
        public IList<IWebElement>? Prices { get; set; }

        [FindsBy(How = How.CssSelector, Using = ".sc-728866e0-3.FBCRN div")]
        public IWebElement? Description { get; set; }

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

        public BikeDetailsPageResult GetBikeDetails()
        {
            Logger.LogDebug("Getting bike details");

            var url = WebDriver.Url;

            Logger.LogTrace($"URL: {url}");

            var name = GetName();

            Logger.LogTrace($"Name: {name}");

            var description = GetDescription();

            Logger.LogTrace($"Description: {description}");

            var videos = GetVideos().ToList();

            Logger.LogTrace($"Videos: {videos.Count}");

            var modelConfigurations = GetModelConfigurations();

            TechnicalSpecifications? technicalSpecifications = null;
            List<ManualDownload>? manualDownloads = null;

            var breadcrumbs = GetBreadcrumbs();

            var model = new Model(url, name, description)
            {
                TechnicalSpecifications = technicalSpecifications,
                ManualDownloads = manualDownloads,
                Configurations = modelConfigurations,
                Videos = videos,
                Breadcrumbs = breadcrumbs
            };

            var bikeDetailsPageResult = new BikeDetailsPageResult(url, model);

            return bikeDetailsPageResult;
        }

        private IEnumerable<Breadcrumb> GetBreadcrumbs()
        {
            Logger.LogDebug("Getting breadcrumbs");

            if (Breadcrumbs is null)
                throw new NullReferenceException($"'{nameof(Breadcrumbs)}' cannot be null");

            var breadcrumbs = new List<Breadcrumb>();

            foreach (var webElement in Breadcrumbs)
            {
                var text = webElement.Text;

                var breadcrumb = new Breadcrumb(text);

                breadcrumbs.Add(breadcrumb);
            }

            return breadcrumbs;
        }

        private string GetName()
        {
            Logger.LogDebug("Getting name");

            if (ProductDetailHeader is null)
                throw new NullReferenceException($"'{nameof(ProductDetailHeader)}' cannot be null");

            var name = ProductDetailHeader.FindElement(By.TagName("h1")).Text.Trim();

            return name;
        }

        private string GetDescription()
        {
            Logger.LogDebug("Getting description");
            var stringBuilder = new StringBuilder();

            var descriptionFromPage = Description?.GetAttribute("innerHTML");
            stringBuilder.Append(descriptionFromPage);

            //var descriptionFromModal = GetDescriptionFromModal();
            //stringBuilder.Append(descriptionFromModal);

            var description = stringBuilder.ToString();

            if (string.IsNullOrWhiteSpace(description))
                throw new Exception($"'{nameof(description)}' should not be null or white space");

            return description;
        }

        private IEnumerable<Video> GetVideos()
        {
            Logger.LogDebug("Getting video links");

            var videoLinks = WebDriver.FindElements(By.CssSelector("a[href*=\"youtube\"]"));

            Logger.LogTrace("Getting 'href' attributes from all video links");

            var videoUrls = videoLinks.Select(x => x.GetAttribute("href"));

            var videos = videoUrls.Select(x => new Video(x));

            return videos;
        }

        private IEnumerable<ModelConfiguration> GetModelConfigurations()
        {
            Logger.LogDebug("Getting model configurations");

            WaitForPageToLoad();

            if (SizeButtons is null)
                throw new NullReferenceException($"'{nameof(SizeButtons)}' cannot be null");

            if (ColorButtons is null)
                throw new NullReferenceException($"'{nameof(ColorButtons)}' cannot be null");

            var modelConfigurations = new List<ModelConfiguration>();

            //var geometryTable = GetGeometryTable();

            AddColorsAndSizesTo(modelConfigurations);
            
            return modelConfigurations;
        }

        private void IterateSizes(string color, IReadOnlyCollection<Image> images, ICollection<ModelConfiguration> modelConfigurations)
        {
            if (SizeButtons is null)
                throw new NullReferenceException($"'{nameof(SizeButtons)}' cannot be null");

            foreach (var sizeButton in SizeButtons)
            {
                Logger.LogTrace($"Scrolling to '{nameof(sizeButton)}'");

                WebDriver.ScrollToElement(sizeButton);

                Logger.LogTrace($"Clicking '{sizeButton.Text}' button");

                sizeButton.Click();

                var size = sizeButton.Text;

                Logger.LogTrace($"Size: {size}");

                //var geometry = GetGeometryFromGeometryTable(size, geometryTable);

                Logger.LogDebug($"Getting configuration for '{color}'/'{size}'");

                var partNumber = GetPartNumber();

                var pricing = GetPricing();

                var modelConfiguration = new ModelConfiguration(
                    partNumber,
                    color,
                    size
                    // TODO: Fix JSON issues with geometry, then uncomment
                    //geometry
                )
                {
                    Pricing = pricing,
                    Images = images
                };

                modelConfigurations.Add(modelConfiguration);
            }
        }

        private void AddColorsAndSizesTo(ICollection<ModelConfiguration> modelConfigurations)
        {
            if (ColorButtons is null)
                throw new NullReferenceException($"'{nameof(ColorButtons)}' cannot be null");

            foreach (var colorButton in ColorButtons)
            {
                Logger.LogTrace("Scrolling to the top of the page");

                WebDriver.ScrollToTopOfPage();

                Logger.LogTrace($"Scrolling to '{nameof(colorButton)}'");

                WebDriver.ScrollToElement(colorButton);

                Logger.LogTrace($"Clicking '{colorButton.GetAttribute("aria-label")}' button");

                try
                {
                    colorButton.Click();
                }
                catch (ElementClickInterceptedException e)
                {
                    DismissReviewSlideUp();

                    colorButton.Click();
                }

                var color = WebDriver
                    .FindElement(By.CssSelector("[data-component=\"color-selection\"] p:last-child")).Text;

                Logger.LogTrace($"Clicked '{color}' button");

                Logger.LogTrace($"Color: {color}");

                // TODO: Move this to a color only loop as we don't need images for each color/size combination (I think)
                var images = GetImages().ToList();

                IterateSizes(color, images, modelConfigurations);
            }
        }

        private void DismissReviewSlideUp()
        {
            var reviewSlideUpLocator = By.CssSelector(".bXNnnC");

            new WebDriverWait(WebDriver, TimeSpan.FromSeconds(5))
                .Until(ExpectedConditions.ElementIsVisible(reviewSlideUpLocator));

            var reviewSlideUp = WebDriver.FindElement(reviewSlideUpLocator);
            reviewSlideUp.FindElement(By.CssSelector(".jUEINs")).Click();

            Logger.LogTrace($"Closed '{nameof(reviewSlideUp)}'");
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
            
            if (!dimensions.Any())
                throw new DimensionsNotFoundException($"No dimensions found in {WebDriver.Url}");

            var geometry = new Geometry(dimensions);

            return geometry;
        }

        private GeometryTable GetGeometryTable()
        {
            Logger.LogDebug("Getting geometry");

            if (TechnicalSpecificationsAccordion is null)
                throw new NullReferenceException($"'{nameof(TechnicalSpecificationsAccordion)}' cannot be null");

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

                var geometryTableRow = GetGeometryTableRow(row, rowIndex, columnHeaderValues);
                geometryTableRows.Add(geometryTableRow);
            }

            if (!geometryTableRows.Any())
                throw new GeometryTableRowsNotFoundException($"No rows found in {WebDriver.Url}");

            var geometryTable = new GeometryTable(geometryTableRows);
            
            return geometryTable;
        }

        private GeometryTableRow GetGeometryTableRow(IWebElement row, int rowIndex,
            IReadOnlyCollection<string> columnHeaderValues)
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

        private void ThrowIfTableLayoutIsUnexpected(IReadOnlyCollection<IWebElement> columns)
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
                        isInvalidValue = int.Parse(attributeValue) > 1;

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

        private IEnumerable<Image> GetImages()
        {
            Logger.LogDebug($"Getting images from '{nameof(Gallery)}'");

            if (Gallery is null)
                throw new NullReferenceException($"'{nameof(Gallery)}' cannot be null");

            var images = new List<Image>();

            var imgElements = Gallery.FindElements(By.TagName("img"));

            Logger.LogTrace($"Found {imgElements.Count} 'img' elements in the '{nameof(Gallery)}' element");

            foreach (var imgElement in imgElements)
            {
                var src = imgElement.GetAttribute("src").Trim();

                Logger.LogTrace($"Adding '{src}' to images");

                var image = new Image(src);

                images.Add(image);
            }

            Logger.LogTrace($"Scraped {images.Count} images from '{nameof(Gallery)}'");

            return images;
        }

        private Pricing GetPricing()
        {
            Logger.LogDebug($"Getting pricing from '{nameof(Prices)}'");

            if (Prices is null)
                throw new NullReferenceException($"'{nameof(Prices)}' cannot be null");

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
            if (ProductDetailHeader is null)
                throw new NullReferenceException($"'{nameof(ProductDetailHeader)}' cannot be null");

            Logger.LogDebug("Getting part number");

            var text = ProductDetailHeader.FindElement(By.CssSelector("p span")).Text;
            var split = text.Split(':', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var partNumber = split.Last().Trim();

            Logger.LogTrace($"Parsed '{partNumber}' from '{text}'");

            return partNumber;
        }

        private string GetDescriptionFromModal()
        {
            Logger.LogDebug("Getting description");

            Logger.LogTrace("Clicking 'Read More' button (to open the description modal)");

            // Open the modal by clicking the "Read More" button
            ReadMoreButton?.Click();

            var modalLocator = this.GetByFrom(nameof(Modal));

            new WebDriverWait(WebDriver, TimeSpan.FromSeconds(5))
                .Until(ExpectedConditions.ElementIsVisible(modalLocator));

            if (Modal is null)
                throw new NullReferenceException($"'{nameof(Modal)}' cannot be null");

            var modalElement = new ModalElement(Logger, WebDriver);

            var description = modalElement.GetDescription();

            return description;
        }
    }
}
