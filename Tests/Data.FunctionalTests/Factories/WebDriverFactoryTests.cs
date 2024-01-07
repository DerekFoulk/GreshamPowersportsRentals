using Data.Factories;
using FluentAssertions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;

namespace Data.FunctionalTests.Factories
{
    public class WebDriverFactoryTests
    {
        [Fact(Skip = "Only run if you need to")]
        public void GetWebDriver_EdgeDriver_ReturnsExpected()
        {
            // Arrange
            var webDriverFactory = new WebDriverFactory();

            // Act
            var webDriver = webDriverFactory.GetWebDriver<EdgeDriver>(TimeSpan.FromSeconds(3));

            try
            {
                // Assert
                webDriver.Should()
                    .BeAssignableTo<IWebDriver>();
                webDriver.Should()
                    .BeOfType<EdgeDriver>();
                webDriver.Should()
                    .NotBeNull();
            }
            finally
            {
                webDriver.Quit();
            }
        }

        [Fact(Skip = "Only run if you need to")]
        public void GetWebDriver_EdgeDriver_Headless_ReturnsExpected()
        {
            // Arrange
            var webDriverFactory = new WebDriverFactory();

            // Act
            var webDriver = webDriverFactory.GetWebDriver<EdgeDriver>(TimeSpan.FromSeconds(3), true);

            try
            {
                // Assert
                webDriver.Should()
                    .BeAssignableTo<IWebDriver>();
                webDriver.Should()
                    .BeOfType<EdgeDriver>();
                webDriver.Should()
                    .NotBeNull();
            }
            finally
            {
                webDriver.Quit();
            }
        }

        [Fact(Skip = "Only run if you need to")]
        public void GetWebDriver_ChromeDriver_ReturnsExpected()
        {
            // Arrange
            var webDriverFactory = new WebDriverFactory();

            // Act
            var webDriver = webDriverFactory.GetWebDriver<ChromeDriver>(TimeSpan.FromSeconds(3));

            try
            {
                // Assert
                webDriver.Should()
                    .BeAssignableTo<IWebDriver>();
                webDriver.Should()
                    .BeOfType<ChromeDriver>();
                webDriver.Should()
                    .NotBeNull();
            }
            finally
            {
                webDriver.Quit();
            }
        }

        [Fact(Skip = "Only run if you need to")]
        public void GetWebDriver_ChromeDriver_Headless_ReturnsExpected()
        {
            // Arrange
            var webDriverFactory = new WebDriverFactory();

            // Act
            var webDriver = webDriverFactory.GetWebDriver<ChromeDriver>(TimeSpan.FromSeconds(3), true);

            try
            {
                // Assert
                webDriver.Should()
                    .BeAssignableTo<IWebDriver>();
                webDriver.Should()
                    .BeOfType<ChromeDriver>();
                webDriver.Should()
                    .NotBeNull();
            }
            finally
            {
                webDriver.Quit();
            }
        }

        [Fact(Skip = "Only run if you need to")]
        public void GetWebDriver_FirefoxDriver_ReturnsExpected()
        {
            // Arrange
            var webDriverFactory = new WebDriverFactory();

            // Act
            var webDriver = webDriverFactory.GetWebDriver<FirefoxDriver>(TimeSpan.FromSeconds(3));

            try
            {
                // Assert
                webDriver.Should()
                    .BeAssignableTo<IWebDriver>();
                webDriver.Should()
                    .BeOfType<FirefoxDriver>();
                webDriver.Should()
                    .NotBeNull();
            }
            finally
            {
                webDriver.Quit();
            }
        }

        [Fact(Skip = "Only run if you need to")]
        public void GetWebDriver_FirefoxDriver_Headless_ReturnsExpected()
        {
            // Arrange
            var webDriverFactory = new WebDriverFactory();

            // Act
            var webDriver = webDriverFactory.GetWebDriver<FirefoxDriver>(TimeSpan.FromSeconds(3), true);

            try
            {
                // Assert
                webDriver.Should()
                    .BeAssignableTo<IWebDriver>();
                webDriver.Should()
                    .BeOfType<FirefoxDriver>();
                webDriver.Should()
                    .NotBeNull();
            }
            finally
            {
                webDriver.Quit();
            }
        }
    }
}
