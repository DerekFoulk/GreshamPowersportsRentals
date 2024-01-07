using BlazorApp.Shared;
using System.Collections.Generic;

namespace Api
{
    public interface IDatastore
    {
        List<Bike> Bikes { get; set; }
        List<Category> Categories { get; set; }
        List<Manufacturer> Manufacturers { get; set; }
        List<Model> Models { get; set; }
        List<Rental> Rentals { get; set; }
    }
}