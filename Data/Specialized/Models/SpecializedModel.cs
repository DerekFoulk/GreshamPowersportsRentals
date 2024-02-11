namespace Data.Specialized.Models
{
    public record SpecializedModel(
        string Url,
        string Name,
        string Description,
        TechnicalSpecifications? TechnicalSpecifications,
        IEnumerable<ManualDownload>? ManualDownloads,
        IEnumerable<ModelConfiguration> Configurations,
        IEnumerable<string>? Videos,
        IEnumerable<string> Breadcrumbs
    )
    {
        public override string ToString()
        {
            return Name;
        }
    }
}
