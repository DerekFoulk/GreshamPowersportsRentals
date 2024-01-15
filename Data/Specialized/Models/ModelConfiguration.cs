namespace Data.Specialized.Models
{
    public record ModelConfiguration(
        string PartNumber,
        Pricing Pricing,
        string Color,
        IEnumerable<string> Images,
        string Size
        // TODO: Fix JSON issues with geometry, then uncomment
        //Geometry Geometry
    )
    {
        public override string ToString()
        {
            return $"#{PartNumber} ({Size})";
        }
    };
}
