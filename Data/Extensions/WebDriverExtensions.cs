using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using ByteSizeLib;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace Data.Extensions
{
    public static class WebDriverExtensions
    {
        public static void ScrollToTopOfPage(this IWebDriver webDriver)
        {
            var js = (IJavaScriptExecutor)webDriver;
            js.ExecuteScript("window.scrollTo(0, 0)");
        }

        public static void ScrollToBottomOfPage(this IWebDriver webDriver)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)webDriver;
            js.ExecuteScript("window.scrollTo(0, document.body.scrollHeight)");
        }

        public static void ScrollToElement(this IWebDriver webDriver, IWebElement webElement)
        {
            new Actions(webDriver)
                .ScrollToElement(webElement)
                .Perform();
        }
        
        public static bool IsElementVisible(this IWebDriver webDriver, By locator)
        {
            try
            {
                var func = ExpectedConditions.ElementIsVisible(locator);
                var webElement = func(webDriver);

                if (webElement is null)
                    return false;

                var isVisible = webElement.Displayed;

                return isVisible;
            }
            catch (Exception e) when (e is NoSuchElementException or StaleElementReferenceException)
            {
                return false;
            }
        }

        public static void CaptureHtmlAndScreenshot(this IWebDriver webDriver, Exception exception, Type type, MethodBase? method, ILogger? logger = null)
        {
            var name = $"{DateTime.Now:yyyyMMddHHmmss}_{exception.GetType().FullName}_{type.FullName}_{method?.Name}";
            CaptureHtmlAndScreenshot(webDriver, name);
        }

        public static void CaptureHtmlAndScreenshot(this IWebDriver webDriver, string name, ILogger? logger = null)
        {
            logger?.LogDebug($"Capturing HTML and screenshot as '{name}'");

            var captureHtmlTask = Task.Run(() =>
            {
                var html = webDriver.PageSource;

                logger?.LogTrace($"Captured '{html.Length}' characters of HTML");

                return html;
            });

            var captureScreenshotTask = Task.Run(() =>
            {
                var screenshot = webDriver.TakeScreenshot();

                logger?.LogTrace($"Captured screenshot ({ByteSize.FromBytes(screenshot.AsByteArray.Length).KiloBytes} kB)");

                return screenshot;
            });

            Task.WaitAll(captureHtmlTask, captureScreenshotTask);

            var tempDirectoryPath = Path.Combine(Path.GetTempPath(), "GreshamPowersportsRentals");
            var directoryPath = Path.Combine(tempDirectoryPath, "SeleniumCaptures", name);
            var directoryInfo = new DirectoryInfo(directoryPath);

            logger?.LogTrace($"Saving capture as '{name}' ('{directoryInfo.FullName}')");

            if (!directoryInfo.Exists)
            {
                logger?.LogTrace($"'{directoryInfo.FullName}' does not exist");

                directoryInfo.Create();

                logger?.LogTrace($"Created '{directoryInfo.FullName}'");
            }

            var saveHtmlTask = Task.Run(() =>
            {
                var html = captureHtmlTask.Result;
                var htmlFilePath = Path.Combine(directoryPath, $"{name}.html");

                File.WriteAllText(htmlFilePath, html);

                logger?.LogTrace($"Saved HTML as '{htmlFilePath}'");
            });

            var saveScreenshotTask = Task.Run(() =>
            {
                var screenshot = captureScreenshotTask.Result;
                var screenshotFilePath = Path.Combine(directoryPath, $"{name}.png");

                screenshot.SaveAsFile(screenshotFilePath);

                logger?.LogTrace($"Saved screenshot as '{screenshotFilePath}'");
            });

            Task.WaitAll(saveHtmlTask, saveScreenshotTask);

            logger?.LogTrace($"Opening '{directoryPath}'");

            OpenDirection(directoryPath);
        }

        public static void SetPosition(this IWebDriver webDriver, int displayOnMonitor, int resolutionX, bool maximize = false)
        {
            var xMultiplier = displayOnMonitor - 1;

            //xMultiplier = xMultiplier < 0 ? 0 : xMultiplier;

            var x = resolutionX * xMultiplier;
            webDriver.Manage().Window.Position = new Point(x, 0);

            if (maximize)
                webDriver.Manage().Window.Maximize();
        }

        public static void DisableJavaScript(this IWebDriver webDriver)
        {
            var currentUrl = webDriver.Url;

            IWebElement toggleElement;

            switch (webDriver)
            {
                case EdgeDriver _:
                    webDriver.Navigate().GoToUrl("edge://settings/content/javascript");
                    toggleElement = webDriver.FindElement(By.CssSelector("input#permission-toggle-row:checked"));
                    break;
                case ChromeDriver _:
                    // TODO: Test
                    webDriver.Navigate().GoToUrl("chrome://settings/content/javascript");
                    toggleElement = webDriver.FindElement(By.CssSelector("cr-toggle#control:checked"));
                    break;
                case FirefoxDriver _:
                    {
                        // TODO: Test
                        webDriver.Navigate().GoToUrl("about:config");

                        var wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(5));
                        var x = wait.Until(x => x.FindElement(By.CssSelector("warningButton")));

                        toggleElement = webDriver.FindElement(By.CssSelector("cr-toggle#control:checked"));
                        break;
                    }
                default:
                    throw new InvalidOperationException();
            }

            toggleElement?.Click();

            webDriver.Navigate().GoToUrl(currentUrl);
        }

        public static void SetTimeouts(this IWebDriver webDriver, TimeSpan pageLoadTimeout,
            TimeSpan asynchronousJavaScriptTimeout, TimeSpan implicitWaitTimeout)
        {
            var timeouts = webDriver.Manage().Timeouts();

            if (!pageLoadTimeout.Equals(timeouts.PageLoad))
                webDriver.Manage().Timeouts().PageLoad = pageLoadTimeout;

            if (!asynchronousJavaScriptTimeout.Equals(timeouts.AsynchronousJavaScript))
                webDriver.Manage().Timeouts().AsynchronousJavaScript = asynchronousJavaScriptTimeout;

            if (!implicitWaitTimeout.Equals(timeouts.ImplicitWait))
                webDriver.Manage().Timeouts().ImplicitWait = implicitWaitTimeout;
        }

        private static void OpenDirection(string directoryPath)
        {
            Process.Start("explorer", directoryPath);
        }

        private static void OpenFile(string directoryPath)
        {
            var startInfo = new ProcessStartInfo(directoryPath)
            {
                UseShellExecute = true
            };

            Process.Start(startInfo);
        }
    }
}
