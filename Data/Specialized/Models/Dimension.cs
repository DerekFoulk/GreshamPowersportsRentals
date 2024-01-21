using System.ComponentModel;
using Data.Specialized.Converters;

namespace Data.Specialized.Models
{
    [TypeConverter(typeof(DimensionConverter))]
    public record Dimension(string Name, string ImageUrl);
}
