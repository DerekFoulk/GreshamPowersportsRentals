using BlazorApp.Shared;

namespace Data.Specialized.Entities
{
    public class ModelConfiguration : Entity
    {
        public ModelConfiguration() { }

        public ModelConfiguration(string partNumber, Pricing pricing, string color, IEnumerable<Image> images, string size, Model model)
        {
            PartNumber = partNumber;
            Pricing = pricing;
            Color = color;
            Images = images;
            Size = size;
            Model = model;
        }

        public string PartNumber { get; set; }
        public Pricing Pricing { get; set; }
        public string Color { get; set; }
        public IEnumerable<Image> Images { get; set; }
        public string Size { get; set; }

        public Model Model { get; set; }

        public override string ToString()
        {
            return $"#{Model.Name} - {Color} ({Size})";
        }
    }
}
