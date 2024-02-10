using Data.Factories;
using Data.Husqvarna.Models;
using Data.Husqvarna.Services;
using Data.Options;
using Divergic.Logging.Xunit;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit.Abstractions;

namespace Data.FunctionalTests.Husqvarna.Services
{
    public class HusqvarnaBicyclesServiceTests : LoggingTestsBase<HusqvarnaBicyclesService>
    {
        public HusqvarnaBicyclesServiceTests(ITestOutputHelper output) : base(output, LogLevel.Trace)
        {
        }

        [Fact]
        public void GetBicycleInfo_ReturnsExpected()
        {
            // Arrange
            var webDriverFactory = new WebDriverFactory();
            var mockOptionsSnapshot = new Mock<IOptionsSnapshot<WebDriverOptions>>();
            var webDriverOptions = new WebDriverOptions
            {
                Headless = false,
                ImplicitWaitInSeconds = 3
            };
            mockOptionsSnapshot.Setup(x => x.Value)
                .Returns(webDriverOptions);
            using var husqvarnaBicyclesService = new HusqvarnaBicyclesService(Logger, mockOptionsSnapshot.Object, webDriverFactory);

            // Act
            var bicycleInfo = husqvarnaBicyclesService.GetBicycleInfo("https://www.husqvarna-bicycles.com/en-us/models/offroad/hard-cross/hard-cross-hc5-2023.7902307544.html");

            // Assert
            BicycleInfoAssertions(bicycleInfo);
        }

        [Fact]
        public void GetBicycleInfos_ReturnsExpected()
        {
            // Arrange
            var webDriverFactory = new WebDriverFactory();
            var mockOptionsSnapshot = new Mock<IOptionsSnapshot<WebDriverOptions>>();
            var webDriverOptions = new WebDriverOptions
            {
                Headless = false,
                ImplicitWaitInSeconds = 3
            };
            mockOptionsSnapshot.Setup(x => x.Value)
                .Returns(webDriverOptions);
            using var husqvarnaBicyclesService = new HusqvarnaBicyclesService(Logger, mockOptionsSnapshot.Object, webDriverFactory);

            // Act
            var bicycleInfos = husqvarnaBicyclesService.GetBicycleInfos();

            // Assert
            bicycleInfos.Should()
                .HaveCount(7)
                .And.AllSatisfy(BicycleInfoAssertions);
        }

        private void BicycleInfoAssertions(HusqvarnaBicycleInfo bicycleInfo)
        {
            bicycleInfo.Name.Should()
                .NotBeNullOrWhiteSpace();
            bicycleInfo.Msrp.Should()
                .BePositive();

            bicycleInfo.Images.LargeImage.Should()
                .NotBeNullOrEmpty();

            bicycleInfo.Description.Heading.Should()
                .NotBeNullOrWhiteSpace();
            bicycleInfo.Description.Text.Should()
                .NotBeNullOrWhiteSpace();

            bicycleInfo.Specifications.Should()
                .BeNull();
        }
    }
}
