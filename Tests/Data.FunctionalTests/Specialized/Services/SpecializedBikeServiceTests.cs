using Data.Factories;
using Data.Options;
using Data.Specialized.Models;
using Data.Specialized.Services;
using Divergic.Logging.Xunit;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit.Abstractions;

namespace Data.FunctionalTests.Specialized.Services
{
    public class SpecializedBikeServiceTests : LoggingTestsBase<SpecializedBikesService>
    {
        private readonly int _maxDepth = AssertionOptions.FormattingOptions.MaxDepth;
        private readonly int _maxLines = AssertionOptions.FormattingOptions.MaxLines;

        public SpecializedBikeServiceTests(ITestOutputHelper output) : base(output, LogLevel.Trace)
        {
            AssertionOptions.FormattingOptions.MaxDepth = 1;
            AssertionOptions.FormattingOptions.MaxLines = int.MaxValue;
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            AssertionOptions.FormattingOptions.MaxDepth = _maxDepth;
            AssertionOptions.FormattingOptions.MaxLines = _maxLines;

            base.Dispose(disposing);
        }

        [Fact]
        public async Task GetBikes_ReturnsExpected()
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
            var specializedBikesService = new SpecializedBikesService(Logger, mockOptionsSnapshot.Object, webDriverFactory);

            // Act
            var models = (await specializedBikesService.GetModelsAsync(7, 7)).ToList();

            // Assert
            models.Should()
                .NotBeNullOrEmpty()
                //.HaveCount(18)
                .And.AllSatisfy(model =>
                {
                    model.Name.Should()
                        .NotBeNullOrWhiteSpace();
                    model.Name.Length.Should()
                        .BeGreaterThan(5)
                        .And.BeLessThan(40, $"[Model #{models.IndexOf(model)}] Long name detected for '{model}' ('{model.Name}') @ '{model.Url}'");
                    
                    model.Description.Should()
                        .NotBeNullOrWhiteSpace();
                    model.Description.Length.Should()
                        .BeGreaterThan(50, $"[Model #{models.IndexOf(model)}] Insufficient description detected for '{model}' ('{model.Description}') @ '{model.Url}'")
                        .And.BeLessThan(750);

                    model.TechnicalSpecifications.Should()
                        .BeEquivalentTo(default(TechnicalSpecifications));

                    model.ManualDownloads.Should()
                        .BeNullOrEmpty();

                    model.Configurations.Count().Should()
                        .BeGreaterThanOrEqualTo(4)
                        .And.BeLessThanOrEqualTo(25);
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

                            // TODO: Fix JSON issues with geometry, then uncomment
                            //configuration.Geometry.Dimensions.Should()
                            //    .NotBeNullOrEmpty($"[Model #{models.IndexOf(model)}] Could not find dimensions for '{model}' @ '{model.Url}'")
                            //    .And.AllSatisfy(dimensionKvp =>
                            //    {
                            //        var dimension = dimensionKvp.Key;
                            //        var dimensionValue = dimensionKvp.Value;

                            //        dimension.Name.Should()
                            //            .NotBeNullOrWhiteSpace();

                            //        dimension.ImageUrl.Should()
                            //            .NotBeNullOrWhiteSpace();
                            //        dimension.ImageUrl.Should()
                            //            .Contain("http");
                            //        dimension.ImageUrl.Should()
                            //            .Contain("/");
                            //        dimension.ImageUrl.Should()
                            //            .Contain(".");

                            //        dimensionValue.Should()
                            //            .NotBeNullOrWhiteSpace();
                            //    })
                            //    .And.HaveCountGreaterOrEqualTo(15)
                            //    .And.HaveCountLessOrEqualTo(25);
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

                    model.Breadcrumbs.Should()
                        .NotBeNullOrEmpty()
                        .And.ContainEquivalentOf("Bikes");
                });
                //.BeGreaterOrEqualTo(305)
                //.And.BeLessThan(315);
        }
    }
}
