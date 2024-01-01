using System;
using System.Collections.Generic;
using System.Linq;
using AutoBogus;
using BlazorApp.Shared;
using BlazorApp.Shared.Extensions;
using Bogus;

namespace Api;

public class FakeDatastore
{
    public FakeDatastore()
    {
        var manufacturers = new AutoFaker<Manufacturer>().Generate(5);
        
        var bikeTypes = Enum.GetValues<BikeType>()
            .Select(x => x.GetDisplayName())
            .ToList();

        var categoryImages = new Dictionary<string, string>
        {
            {
                BikeType.Electric.GetDisplayName(),
                "https://s7g10.scene7.com/is/image/ktm/husqvarna%20e%20bicycles%20Hard%20Cross%208:Large?wid=1260&hei=718&dpr=off"
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
                "https://th.bing.com/th/id/OIP.VehzIJabvNAkaWtcFHXmWgAAAA?rs=1&pid=ImgDetMain"
            }
        };

        var categories = new AutoFaker<Category>()
            .RuleFor(x => x.Name, f => bikeTypes[f.IndexFaker])
            .RuleFor(x => x.Image, (f, x) => categoryImages[x.Name])
            .Generate(bikeTypes.Count);
        var bikes = new AutoFaker<Bike>()
            .RuleFor(x => x.Manufacturer, f => f.PickRandom(manufacturers))
            .RuleFor(x => x.Category, f => f.PickRandom(categories))
            .RuleFor(x => x.Description, (f, x) => GetRandomDescriptionHtml(f, x))
            .RuleFor(x => x.PricePerHour, f => 1M)
            .Generate(15);
        var rentals = new AutoFaker<Rental>()
            .RuleFor(x => x.Bikes, f => f.PickRandom(bikes, f.Random.Number(1, 5)))
            .RuleFor(x => x.StartDate,
                f => f.Date.BetweenDateOnly(DateOnly.FromDateTime(DateTime.Today),
                    DateOnly.FromDateTime(DateTime.Today.AddDays(7))))
            .RuleFor(x => x.StartTime,
                f => f.Date.BetweenTimeOnly(TimeOnly.Parse("9:00 AM"), TimeOnly.Parse("5:00 PM")))
            .RuleFor(x => x.EndDate, (f, x) => f.Date.SoonDateOnly(7, x.StartDate))
            .RuleFor(x => x.EndTime,
                f => f.Date.BetweenTimeOnly(TimeOnly.Parse("9:00 AM"), TimeOnly.Parse("5:00 PM")))
            .Generate(10);

        Manufacturers = manufacturers;
        Categories = categories;
        Bikes = bikes;
        Rentals = rentals;
    }

    public List<Bike> Bikes { get; set; }
    public List<Category> Categories { get; set; }
    public List<Manufacturer> Manufacturers { get; set; }
    public List<Rental> Rentals { get; set; }

    public string GetRandomDescriptionHtml(Faker faker, Bike bike)
    {
        var html =
            "<p>Looking for a mountain bike perfect for Duthie bike park, or for peddling the Thrilla or Pipeline gravel trails? Look no further.</p><p>Excuse the brag, but we’ve really outdone ourselves here. Proving that true trail-taming capability doesn’t have to come at an intimidating price, the Rockhopper Expert is the culmination of 30 years of redefining the relationship between value and performance.</p><p>Start with a painstakingly engineered Premium A1 Aluminum frame, add in modern geometry that keeps one eye on efficiency and the other on confident capability and you have the rock-solid foundation of our best Rockhopper yet.</p><p>Pair that with a parts list that just doesn’t quit (...can we get a shoutout for RockShox’s Judy SoloAir fork and SRAM’s always-in-charge Level hydraulic disc brakes?) and you have a tubeless-ready, SRAM Eagle 1x12-equipped Rockhopper that’s absolutely prepared to fly.</p><ul><li>A heart of gold, presented in our lightweight yet durable Premium A1 Aluminum, the Rockhopper’s butted aluminum frame features hydroformed top and downtubes in order to keep weight low and strength high, all while providing increased standover clearance, slick internal cable routing and dropper-post compatibility.</li><li>With the goal of making sure that the Rockhopper offers both the best fit and best performance for every person, no matter their measure, we’ve paired each Rockhopper frame size with the optimal wheel size. The result is a Rockhopper to fit every rider and the assurance that every Rockhopper is rolling on the best-handling wheel for its frame size.</li><li>Beautifully beastly, SRAM Level hydraulic disc brakes pack a mean bite but deliver their power with a refined, intuitive feel to make for a package that punches way above its pay grade.</li><li>Keep things simple with a durable 1x Sram SX drivetrain that offers plenty of range.</li><li>Pair dead-reliable hubs from Formula with our 25-millimeter internal diameter, hookless Stout rims for a wheelset hat-trick: strong, light and durable.</li></ul>";

        return html;
    }
}