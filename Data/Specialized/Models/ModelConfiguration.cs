namespace Data.Specialized.Models
{
    public record ModelConfiguration(string PartNumber, Pricing Pricing, string Color, IEnumerable<string> Images, string Size, Geometry Geometry);
}
