using Data.Factories;
using Data.Husqvarna.Services;
using Divergic.Logging.Xunit;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Data.FunctionalTests.Husqvarna.Services
{
    public class HusqvarnaBicyclesServiceTests : LoggingTestsBase<HusqvarnaBicyclesService>
    {
        public HusqvarnaBicyclesServiceTests(ITestOutputHelper output) : base(output, LogLevel.Debug)
        {
        }

        [Fact]
        public void GetBicycles_ReturnsExpected()
        {
            // Arrange
            var webDriverFactory = new WebDriverFactory();
            var husqvarnaBicyclesService = new HusqvarnaBicyclesService(Logger, webDriverFactory);

            // Act
            var models = husqvarnaBicyclesService.GetBicycleInfos();

            // Assert
            models.Should()
                .HaveCount(8)
                .And.AllSatisfy(bicycleInfo =>
                {
                    bicycleInfo.Name.Should().NotBeNullOrWhiteSpace();
                    bicycleInfo.Msrp.Should().BeGreaterThan(0);

                    bicycleInfo.Images.LargeImage.Should()
                        .NotBeNullOrEmpty();

                    bicycleInfo.Description.Heading.Should()
                        .NotBeNullOrWhiteSpace();
                    bicycleInfo.Description.Text.Should()
                        .NotBeNullOrWhiteSpace();

                    bicycleInfo.Specifications.Should()
                        .BeNull();
                });
        }
    }
}
