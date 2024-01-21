using System.ComponentModel;
using Data.Specialized.Converters;

namespace Data.Specialized.Entities
{
    [TypeConverter(typeof(DimensionConverter))]
    public record Dimension(string Name, string ImageUrl);
}
