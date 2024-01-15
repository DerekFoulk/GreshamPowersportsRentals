using System.Drawing;
using System.Reflection;
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

        public static void CaptureHtmlAndScreenshot(this IWebDriver webDriver, Exception exception, Type type, MethodBase? method, bool clean = false)
        {
            var name = $"{DateTime.Now:yyyyMMddHHmmss}_{exception.GetType().FullName}_{type.FullName}_{method?.Name}";
            CaptureHtmlAndScreenshot(webDriver, name);
        }

        public static void CaptureHtmlAndScreenshot(this IWebDriver webDriver, string name)
        {
            var captureHtmlTask = Task.Run(() =>
            {
                var html = webDriver.PageSource;

                return html;
            });

            var captureScreenshotTask = Task.Run(() =>
            {
                var screenshot = webDriver.TakeScreenshot();

                return screenshot;
            });

            Task.WaitAll(captureHtmlTask, captureScreenshotTask);

            var directoryPath = $"SeleniumCaptures/{name}/";
            var directoryInfo = new DirectoryInfo(directoryPath);

            if (!directoryInfo.Exists)
                directoryInfo.Create();

            var saveHtmlTask = Task.Run(() =>
            {
                var html = captureHtmlTask.Result;
                var htmlFilePath = Path.Combine(directoryPath, $"{name}.html");

                File.WriteAllText(htmlFilePath, html);
            });

            var saveScreenshotTask = Task.Run(() =>
            {
                var screenshot = captureScreenshotTask.Result;
                var screenshotFilePath = Path.Combine(directoryPath, $"{name}.png");

                screenshot.SaveAsFile(screenshotFilePath);
            });

            Task.WaitAll(saveHtmlTask, saveScreenshotTask);
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
    }
}
