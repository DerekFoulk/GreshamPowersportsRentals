using System.ComponentModel.DataAnnotations;

namespace BlazorApp.Shared;

public enum BikeType
{
    [Display(Name = "E-Bikes")]
    Electric,
    [Display(Name = "Mountain Bikes")]
    Mountain,
    [Display(Name = "Road Bikes")]
    Road,
    [Display(Name = "Demo Bikes")]
    Demo,
    [Display(Name = "Active Bikes")]
    Active,
    [Display(Name = "Kids' Bikes")]
    Kids
}