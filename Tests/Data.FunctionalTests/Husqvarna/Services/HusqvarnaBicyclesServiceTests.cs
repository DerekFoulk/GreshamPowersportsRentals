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
        public void GetBicycles_ReturnsExpected()
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
            var husqvarnaBicyclesService = new HusqvarnaBicyclesService(Logger, mockOptionsSnapshot.Object, webDriverFactory);

            // Act
            var models = husqvarnaBicyclesService.GetBicycleInfos();

            // Assert
            models.Should()
                .HaveCount(8)
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
