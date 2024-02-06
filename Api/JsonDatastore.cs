using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using AutoBogus;
using BlazorApp.Shared;
using BlazorApp.Shared.Extensions;
using Data.Specialized.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Model = BlazorApp.Shared.Model;

namespace Api
{
    public class JsonDatastore : IDatastore
    {
        private readonly ILogger<JsonDatastore> _logger;
        private readonly Stopwatch _stopwatch = new();
        
        public List<Bike> Bikes { get; set; }
        public List<Category> Categories { get; set; }
        public List<Manufacturer> Manufacturers { get; set; }
        public List<Model> Models { get; set; }
        public List<Rental> Rentals { get; set; }

        public JsonDatastore(ILogger<JsonDatastore> logger)
        {
            _logger = logger;

            _stopwatch.Start();

            var bikeTypeDisplayNames = Enum.GetValues<BikeType>()
                .Select(x => x.GetDisplayName())
                .ToList();

            var categories = new AutoFaker<Category>()
                .RuleFor(x => x.Name, f => bikeTypeDisplayNames[f.IndexFaker])
                .RuleFor(x => x.Image, (f, x) => GetCategoryImage(x))
                .Generate(bikeTypeDisplayNames.Count);

            Categories = categories;

            var manufacturers = new List<Manufacturer>
            {
                new("Specialized")
                {
                    Id = Guid.NewGuid()
                }
            };

            Manufacturers = manufacturers;

            var models = GetModelsFromJsonFiles();

            Models = models;

            var bikes = models.SelectMany(x => x.Bikes).Distinct().ToList();

            Bikes = bikes;

            var rentals = new AutoFaker<Rental>()
                .RuleFor(x => x.StartDate,
                    f => f.Date.BetweenDateOnly(DateOnly.FromDateTime(DateTime.Today),
                        DateOnly.FromDateTime(DateTime.Today.AddDays(7))))
                .RuleFor(x => x.StartTime,
                    f => f.Date.BetweenTimeOnly(TimeOnly.Parse("9:00 AM"), TimeOnly.Parse("5:00 PM")))
                .RuleFor(x => x.EndDate, (f, x) => f.Date.SoonDateOnly(7, x.StartDate))
                .RuleFor(x => x.EndTime,
                    f => f.Date.BetweenTimeOnly(TimeOnly.Parse("9:00 AM"), TimeOnly.Parse("5:00 PM")))
                .RuleFor(x => x.Bikes, f => f.PickRandom(bikes, f.Random.Number(1, 4)).ToList())
                .Generate(10);
            
            Rentals = rentals;

            _stopwatch.Stop();

            _logger.LogDebug($"Finished loading JSON data after '{_stopwatch.Elapsed}'");
        }

        private List<Model> GetModelsFromJsonFiles()
        {
            var specializedModels = GetSpecializedModels();

            return specializedModels;
        }

        private List<Model> GetSpecializedModels()
        {
            using var streamReader = File.OpenText("Specialized.json");
            var serializer = new JsonSerializer();
            var specializedModels = serializer.Deserialize(streamReader, typeof(List<Data.Specialized.Models.Model>)) as List<Data.Specialized.Models.Model>;

            if (specializedModels is null || !specializedModels.Any())
                throw new NullReferenceException($"'{nameof(specializedModels)}' cannot be null");

            var models = new List<Model>();

            foreach (var specializedModel in specializedModels)
            {
                // TODO: Remove when scraper is working properly
                if (specializedModel.Name.Contains("Frameset", StringComparison.OrdinalIgnoreCase))
                    continue;

                var model = ConvertToModel(specializedModel);

                models.Add(model);
            }

            return models;
        }

        private Model ConvertToModel(Data.Specialized.Models.Model specializedModel)
        {
            var id = Guid.NewGuid();
            var manufacturer =
                Manufacturers.Single(x => x.Name.Equals("Specialized", StringComparison.OrdinalIgnoreCase));
            var name = specializedModel.Name;
            var category = GetCategory(specializedModel);
            var bikeType = Enum.GetValues<BikeType>().Single(x => category.Name.Contains(x.GetDisplayName()));
            var description = specializedModel.Description;

            var averageMsrp = specializedModel.Configurations.Select(x => x.Pricing.Msrp).Average();
            var pricePerHour = GetPricePerHour(averageMsrp);
            var pricePerDay = GetPricePerDay(pricePerHour);
            var pricePerWeek = GetPricePerWeek(pricePerDay);

            var bikes = new List<Bike>();

            var model = new Model(id, manufacturer, name, bikeType, category, description, pricePerHour, pricePerDay, pricePerWeek, bikes);

            var configurations = specializedModel.Configurations;

            foreach (var configuration in configurations)
            {
                var bike = ConvertToBike(configuration, model);

                bikes.Add(bike);
            }

            return model;
        }

        private Bike ConvertToBike(ModelConfiguration configuration, Model model)
        {
            var bike = new Bike(model, configuration.Size, configuration.Color, configuration.Images)
            {
                Id = Guid.NewGuid(),
                IsAvailable = true
            };

            return bike;
        }

        private decimal GetPricePerWeek(decimal pricePerDay)
        {
            var pricePerWeek = pricePerDay * 7;

            pricePerWeek *= 0.75M;

            pricePerWeek = Math.Round(pricePerWeek / 5) * 5;

            return pricePerWeek;
        }

        private decimal GetPricePerDay(decimal pricePerHour)
        {
            var pricePerDay = pricePerHour * 24;

            pricePerDay = Math.Round(pricePerDay / 5) * 5;

            return pricePerDay;
        }

        private decimal GetPricePerHour(decimal averageMsrp)
        {
            var totalHoursInAYear = (decimal)TimeSpan.FromDays(90).TotalHours;
            var pricePerHour = averageMsrp / totalHoursInAYear;

            return pricePerHour;
        }

        private Category GetCategory(Data.Specialized.Models.Model specializedModel)
        {
            foreach (var category in Categories)
            {
                if (specializedModel.Breadcrumbs.Contains(category.Name, StringComparer.OrdinalIgnoreCase))
                {
                    return category;
                }
            }

            return Categories.Single(x => x.Name.Equals("Demo Bikes", StringComparison.OrdinalIgnoreCase));

            //throw new InvalidOperationException($"Cannot determine category for '{specializedModel}' from '{string.Join(", ", specializedModel.Breadcrumbs)}'");
        }

        private string GetCategoryImage(Category category)
        {
            var categoryImages = new Dictionary<string, string>
            {
                {
                    BikeType.Electric.GetDisplayName(),
                    "https://s7g10.scene7.com/is/image/ktm/husqvarna%20e%20bicycles%20Hard%20Cross%208:Small?wid=600&hei=450&dpr=off"
                },
                {
                    BikeType.Mountain.GetDisplayName(),
                    "https://keyassets.timeincuk.net/inspirewp/live/wp-content/uploads/sites/11/2022/04/RZ93809-630x358.jpg"
                },
                {
                    BikeType.Road.GetDisplayName(),
                    "https://dqh479dn9vg99.cloudfront.net/wp-content/uploads/sites/9/2023/05/specialized-allez-2023-review-1-970x646.jpg"
                },
                {
                    BikeType.Demo.GetDisplayName(),
                    "https://mbaction.com/wp-content/uploads/2021/10/SJ_EVO_Alloy_Hero_Front_3_Quarter-1536x1024.jpg"
                },
                {
                    BikeType.Active.GetDisplayName(),
                    "https://images.immediate.co.uk/production/volatile/sites/21/2022/08/Specialized-Turbo-Tero-3.0-02-d86fbd2.jpg?quality=90&resize=620%2C413"
                },
                {
                    BikeType.Kids.GetDisplayName(),
                    "https://images.singletracks.com/blog/wp-content/uploads/2021/10/specialized_riprock_kids_bike-05.jpg"
                }
            };

            var image = categoryImages.Single(x => category.Name.Contains(x.Key)).Value;

            return image;
        }
    }
}
