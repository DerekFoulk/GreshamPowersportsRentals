namespace Data.Specialized.Models
{
    public record Model(string Name, string Description, TechnicalSpecifications? TechnicalSpecifications, IEnumerable<ManualDownload>? ManualDownloads, IEnumerable<ModelConfiguration> Configurations, IEnumerable<string>? Videos);
}
