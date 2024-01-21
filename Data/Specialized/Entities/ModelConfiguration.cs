using BlazorApp.Shared;

namespace Data.Specialized.Entities
{
    public class ModelConfiguration : Entity
    {
        public ModelConfiguration(string partNumber, string color, string size)
        {
            PartNumber = partNumber;
            Color = color;
            Size = size;
        }

        public string PartNumber { get; set; }
        public Pricing? Pricing { get; set; }
        public string Color { get; set; }
        public IEnumerable<Image>? Images { get; set; }
        public string Size { get; set; }

        public override string ToString()
        {
            return $"#{PartNumber} ({Size})";
        }
    };
}
