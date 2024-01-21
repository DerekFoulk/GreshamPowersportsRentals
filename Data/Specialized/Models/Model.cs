using BlazorApp.Shared;

namespace Data.Specialized.Models
{
    public class Model : Entity
    {
        public Model(string url, string name, string description)
        {
            Url = url;
            Name = name;
            Description = description;
        }

        public string Url { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public TechnicalSpecifications? TechnicalSpecifications { get; set; }
        public IEnumerable<ManualDownload>? ManualDownloads { get; set; }
        public IEnumerable<ModelConfiguration>? Configurations { get; set; }
        public IEnumerable<Video>? Videos { get; set; }
        public IEnumerable<Breadcrumb>? Breadcrumbs { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
