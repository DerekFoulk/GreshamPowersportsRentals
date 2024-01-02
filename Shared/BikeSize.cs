using System.ComponentModel.DataAnnotations;

namespace BlazorApp.Shared;

public enum BikeSize
{
    [Display(Name = "S")]
    Small,
    [Display(Name = "M")]
    Medium,
    [Display(Name = "L")]
    Large,
    [Display(Name = "XL")]
    XLarge,
    [Display(Name = "XXL")]
    // ReSharper disable once InconsistentNaming
    XXLarge
}