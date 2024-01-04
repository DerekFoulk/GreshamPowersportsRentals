using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoBogus;
using BlazorApp.Shared;
using BlazorApp.Shared.Extensions;

namespace Api;

public class FakeDatastore
{
    public FakeDatastore()
    {
        var manufacturers = new AutoFaker<Manufacturer>().Generate(5);

        var bikeTypeDisplayNames = Enum.GetValues<BikeType>()
            .Select(x => x.GetDisplayName())
            .ToList();

        var categories = new AutoFaker<Category>()
            .RuleFor(x => x.Name, f => bikeTypeDisplayNames[f.IndexFaker])
            .RuleFor(x => x.Image, (f, x) => GetCategoryImage(x))
            .Generate(bikeTypeDisplayNames.Count);
        
        var bikeFaker = new AutoFaker<Bike>();
        
        var models = new AutoFaker<Model>()
            .RuleFor(x => x.Manufacturer, f => f.PickRandom(manufacturers))
            .RuleFor(x => x.Category, f => f.PickRandom(categories))
            .RuleFor(x => x.Description, (f, x) => $"A description of the \"{x.Name}\", manufactured by {x.Manufacturer.Name}. This {x.Type.GetDisplayName().ToLower()} is listed in the \"{x.Category.Name}\" category. It can be rented for {x.PricePerHour:C}/hr ({x.PricePerDay:C}/day) from Gresham Powersports.{Environment.NewLine}{Environment.NewLine}<strong>Available Sizes</strong><ul>{GetAvailableSizesHtml(x)}</ul>")
            .RuleFor(x => x.PricePerHour, f => f.Finance.Amount(5, 75))
            .RuleFor(x => x.Bikes, (f, x) => bikeFaker.Generate(f.Random.Number(1, 5)))
            .Generate(15);
        
        var bikes = models.SelectMany(x => x.Bikes).ToList();

        var rentals = new AutoFaker<Rental>()
            .RuleFor(x => x.StartDate,
                f => f.Date.BetweenDateOnly(DateOnly.FromDateTime(DateTime.Today),
                    DateOnly.FromDateTime(DateTime.Today.AddDays(7))))
            .RuleFor(x => x.StartTime,
                f => f.Date.BetweenTimeOnly(TimeOnly.Parse("9:00 AM"), TimeOnly.Parse("5:00 PM")))
            .RuleFor(x => x.EndDate, (f, x) => f.Date.SoonDateOnly(7, x.StartDate))
            .RuleFor(x => x.EndTime,
                f => f.Date.BetweenTimeOnly(TimeOnly.Parse("9:00 AM"), TimeOnly.Parse("5:00 PM")))
            .RuleFor(x => x.Bikes, f => f.PickRandom(bikes, f.Random.Number(1, 4)))
            .Generate(10);

        Bikes = bikes;
        Categories = categories;
        Manufacturers = manufacturers;
        Models = models;
        Rentals = rentals;
    }

    private string GetAvailableSizesHtml(Model model)
    {
        var bikes = model.Bikes;
        var bikeSizes = bikes.Select(x => x.Size).Distinct();

        var stringBuilder = new StringBuilder();

        foreach (var bikeSize in bikeSizes)
        {
            var displayName = bikeSize.GetDisplayName();
            stringBuilder.AppendLine($"<li>{displayName}</li>");
        }

        var html = stringBuilder.ToString();

        return html;
    }

    public List<Bike> Bikes { get; set; }
    public List<Category> Categories { get; set; }
    public List<Manufacturer> Manufacturers { get; set; }
    public List<Model> Models { get; set; }
    public List<Rental> Rentals { get; set; }

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
            }
        };

        var image = categoryImages.Single(x => category.Name.Contains(x.Key)).Value;

        return image;
    }
}