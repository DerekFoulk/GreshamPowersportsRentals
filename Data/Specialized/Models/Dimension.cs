using System.ComponentModel;

namespace Data.Specialized.Models
{
    [TypeConverter(typeof(DimensionConverter))]
    public record Dimension(string Name, string ImageUrl);
}
