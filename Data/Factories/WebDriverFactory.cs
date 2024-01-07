using System.Drawing;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using WebDriverManager;
using WebDriverManager.DriverConfigs;
using WebDriverManager.DriverConfigs.Impl;

namespace Data.Factories
{
    public class WebDriverFactory : IWebDriverFactory
    {
        public IWebDriver GetWebDriver<TWebDriver>(TimeSpan implicitWait, bool headless = false) where TWebDriver : IWebDriver
        {
            IDriverConfig driverConfig;

            if (typeof(TWebDriver) == typeof(EdgeDriver))
                driverConfig = new EdgeConfig();
            else if (typeof(TWebDriver) == typeof(ChromeDriver))
                driverConfig = new ChromeConfig();
            else if (typeof(TWebDriver) == typeof(FirefoxDriver))
                driverConfig = new FirefoxConfig();
            else
                throw new ArgumentOutOfRangeException(nameof(TWebDriver), typeof(TWebDriver), null);

            new DriverManager().SetUpDriver(driverConfig);

            IWebDriver webDriver;
            if (driverConfig is EdgeConfig)
            {
                var edgeOptions = new EdgeOptions();

                if (headless)
                    edgeOptions.AddArguments(new List<string>
                    {
                        "headless"
                    });

                webDriver = new EdgeDriver(edgeOptions);
            }
            else if (driverConfig is ChromeConfig)
            {
                var chromeOptions = new ChromeOptions();
                
                if (headless)
                    chromeOptions.AddArguments(new List<string>
                    {
                        "headless"
                    });
                
                webDriver = new ChromeDriver(chromeOptions);
            }
            else if (driverConfig is FirefoxConfig)
            {
                var firefoxOptions = new FirefoxOptions();

                if (headless)
                    firefoxOptions.AddArguments(new List<string>
                    {
                        "-headless"
                    });
                
                webDriver = new FirefoxDriver(firefoxOptions);
            }
            else
                throw new ArgumentOutOfRangeException(nameof(driverConfig), driverConfig, null);

            webDriver.Manage().Window.Position = new Point(2560, 0);
            //webDriver.Manage().Window.Size = new Size(1920, 1080);
            webDriver.Manage().Window.Maximize();
            webDriver.Manage().Timeouts().ImplicitWait = implicitWait;

            return webDriver;
        }
    }
}
