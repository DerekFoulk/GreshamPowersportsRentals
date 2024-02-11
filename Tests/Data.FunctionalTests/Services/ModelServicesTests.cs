using AutoMapper;
using BlazorApp.Shared.Models;
using Data.Factories;
using Data.Husqvarna.Services;
using Data.Options;
using Data.Profiles;
using Data.Services;
using Data.Specialized.Services;
using Divergic.Logging.Xunit;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit.Abstractions;

namespace Data.FunctionalTests.Services
{
    public class ModelServicesTests : LoggingTestsBase<ModelsService>
    {
        private readonly ITestOutputHelper _output;

        public ModelServicesTests(ITestOutputHelper output) : base(output, LogLevel.Trace)
        {
            _output = output;
        }

        [Fact]
        public async Task GetModelsAsync_ReturnsExpected()
        {
            // Arrange
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });
            var mapper = new Mapper(mapperConfiguration);

            var husqvarnaBicyclesServiceLogger = _output.BuildLoggerFor<HusqvarnaBicyclesService>();
            var optionsSnapshotMock = new Mock<IOptionsSnapshot<WebDriverOptions>>();
            var webDriverOptions = new WebDriverOptions
            {
                Headless = false,
                ImplicitWaitInSeconds = 3
            };
            optionsSnapshotMock.Setup(x => x.Value)
                .Returns(webDriverOptions);
            var webDriverFactory = new WebDriverFactory();
            using var husqvarnaBicyclesService = new HusqvarnaBicyclesService(husqvarnaBicyclesServiceLogger, optionsSnapshotMock.Object, webDriverFactory);
            
            var specializedBikesServiceMock = new Mock<ISpecializedBikesService>();

            var modelsService = new ModelsService(mapper, husqvarnaBicyclesService, specializedBikesServiceMock.Object);

            // Act
            var models = await modelsService.GetModelsAsync();
            
            // Assert
            models.Should()
                .AllBeOfType<Model>()
                .And.NotBeNullOrEmpty()
                .And.AllSatisfy(ModelAssertions);
        }

        private void ModelAssertions(Model model)
        {
            model.Id.Should()
                .NotBeEmpty()
                .And.NotBe(default(Guid));

            ManufacturerAssertions(model.Manufacturer);

            model.Name.Should()
                .NotBeNullOrWhiteSpace();
            
            //model.Type.Should();
            
            CategoryAssertions(model.Category);

            model.Description.Should()
                .NotBeNullOrWhiteSpace();
            model.PricePerHour.Should()
                .BePositive();
            model.PricePerDay.Should()
                .BePositive()
                .And.Be(RoundToNearestFiveDollars(model.PricePerHour * 24M));
            model.PricePerWeek.Should()
                .BePositive()
                .And.Be(RoundToNearestFiveDollars((model.PricePerDay * 7M) * 0.75M));

            model.Bikes.Should()
                .AllSatisfy(BikeAssertions);

        }

        private decimal RoundToNearestFiveDollars(decimal val)
        {
            var ret = Math.Round(val / 5) * 5;

            return ret;
        }

        private void BikeAssertions(Bike bike)
        {
            bike.Should()
                .BeOfType<Bike>()
                .And.NotBeNull();

            bike.Id.Should()
                .NotBeEmpty()
                .And.NotBe(default(Guid));

            bike.Model.Should()
                .BeOfType<Model>()
                .And.NotBeNull();
            
            // TODO: Add check that this Model is equal to the Model that contains it

            bike.Size.Should()
                .NotBeNullOrWhiteSpace();

            bike.Color.Should()
                .NotBeNullOrWhiteSpace();

            bike.Images.Should()
                .AllSatisfy(ImageAssertions);
        }

        private void ImageAssertions(string image)
        {
            image.Should()
                .NotBeNullOrWhiteSpace();
        }

        private void CategoryAssertions(Category category)
        {
            category.Should()
                .BeOfType<Category>()
                .And.NotBeNull();
        }

        private void ManufacturerAssertions(Manufacturer manufacturer)
        {
            manufacturer.Should()
                .BeOfType<Manufacturer>()
                .And.NotBeNull();
        }
    }
}
