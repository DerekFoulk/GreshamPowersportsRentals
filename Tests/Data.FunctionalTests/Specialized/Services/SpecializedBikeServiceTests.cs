using Data.Factories;
using Data.Specialized.Models;
using Data.Specialized.Services;
using Divergic.Logging.Xunit;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;
using static System.Net.Mime.MediaTypeNames;

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
            var models = specializedBikesService.GetBikes().ToList();

            // Assert
            models.Should()
                .AllSatisfy(model =>
                {
                    model.Name.Should()
                        .NotBeNullOrWhiteSpace();
                    model.Name.Length.Should()
                        .BeGreaterThan(5)
                        .And.BeLessThan(25);
                    
                    model.Description.Should()
                        .NotBeNullOrWhiteSpace();
                    model.Description.Length.Should()
                        .BeGreaterThan(50)
                        .And.BeLessThan(750);

                    model.TechnicalSpecifications.Should()
                        .BeEquivalentTo(default(TechnicalSpecifications));

                    model.ManualDownloads.Should()
                        .BeNullOrEmpty();

                    model.Configurations.Count().Should()
                        .BeGreaterThanOrEqualTo(4)
                        .And.BeLessThanOrEqualTo(12);
                    model.Configurations.Should()
                        .AllSatisfy(configuration =>
                        {
                            configuration.PartNumber.Should()
                                .NotBeNullOrWhiteSpace();
                            configuration.PartNumber.Should()
                                .MatchRegex("[0-9]{5}-[0-9]{4}");

                            configuration.Pricing.Msrp.Should()
                                .NotBe(0);
                            configuration.Pricing.Price.Should()
                                .NotBe(0);
                            configuration.Pricing.Msrp.Should()
                                .BeGreaterThanOrEqualTo(configuration.Pricing.Price);

                            configuration.Color.Should()
                                .NotBeNullOrWhiteSpace();

                            configuration.Images.Should()
                                .NotBeNullOrEmpty();
                            configuration.Images.Count().Should()
                                .BeGreaterThanOrEqualTo(1)
                                .And.BeLessThanOrEqualTo(20);

                            // TODO: Redo this better
                            configuration.Images.Should()
                                .AllSatisfy(image =>
                                {
                                    image.Should()
                                        .NotBeNullOrWhiteSpace();
                                    image.Should()
                                        .Contain("http");
                                    image.Should()
                                        .Contain("/");
                                    image.Should()
                                        .Contain(".");
                                });

                            configuration.Size.Should()
                                .NotBeNullOrWhiteSpace();

                            configuration.Geometry.Dimensions.Should()
                                .NotBeNullOrEmpty()
                                .And.AllSatisfy(dimensionKvp =>
                                {
                                    var dimension = dimensionKvp.Key;
                                    var dimensionValue = dimensionKvp.Value;

                                    dimension.Name.Should()
                                        .NotBeNullOrWhiteSpace();

                                    dimension.ImageUrl.Should()
                                        .NotBeNullOrWhiteSpace();
                                    dimension.ImageUrl.Should()
                                        .Contain("http");
                                    dimension.ImageUrl.Should()
                                        .Contain("/");
                                    dimension.ImageUrl.Should()
                                        .Contain(".");

                                    dimensionValue.Should()
                                        .NotBeNullOrWhiteSpace();
                                })
                                .And.HaveCountGreaterOrEqualTo(21);
                        });

                    if (model.Videos?.Any() == true)
                    {
                        Logger.LogInformation($"Model ('{model.Name}') contains {model.Videos.Count()} videos");

                        model.Videos.Should()
                            .AllSatisfy(video =>
                            {
                                video.Should()
                                    .NotBeNullOrWhiteSpace();
                                video.Should()
                                    .Contain("http");
                                video.Should()
                                    .Contain("/");
                                video.Should()
                                    .Contain(".");
                            });
                    }
                });
                //.BeGreaterOrEqualTo(305)
                //.And.BeLessThan(315);
        }
    }
}
