namespace Data.Husqvarna.Models
{
    public record HusqvarnaBicycleInfo(string Name, decimal Msrp, HusqvarnaImages Images,
        HusqvarnaDescription Description, HusqvarnaSpecifications Specifications);
}
