using Data.Factories;
using Data.Specialized.Services;
using Divergic.Logging.Xunit;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Data.FunctionalTests.Specialized.Services
{
    public class SpecializedBikeServiceTests : LoggingTestsBase<SpecializedBikesService>
    {
        public SpecializedBikeServiceTests(ITestOutputHelper output) : base(output, LogLevel.Trace)
        {
        }

        [Fact]
        public void GetBikes_ReturnsExpected()
        {
            // Arrange
            var webDriverFactory = new WebDriverFactory();
            var specializedBikesService = new SpecializedBikesService(Logger, webDriverFactory);
            
            // Act
            var bikeNames = specializedBikesService.GetBikes().ToList();

            // Assert
            bikeNames.Count.Should()
                .BeGreaterOrEqualTo(305)
                .And.BeLessThan(315);
        }
    }
}
