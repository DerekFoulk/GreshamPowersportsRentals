using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AutoBogus;
using AutoMapper;
using BlazorApp.Shared.Extensions;
using BlazorApp.Shared.Models;
using Microsoft.Extensions.Logging;

namespace BlazorApp.Shared
{
    public class JsonDatastore : IDatastore
    {
        private readonly ILogger<JsonDatastore> _logger;
        private readonly IMapper _mapper;
        private readonly Stopwatch _stopwatch = new();
        
        public List<Bike> Bikes { get; set; }
        public List<Category> Categories { get; set; }
        public List<Manufacturer> Manufacturers { get; set; }
        public List<Model> Models { get; set; }
        public List<Rental> Rentals { get; set; }

        public JsonDatastore(ILogger<JsonDatastore> logger, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;

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
            var models = new List<Model>();

            return models;
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
