using BlazorApp.Shared;

namespace Data.Specialized.Models
{
    public class ManualDownload : Entity
    {
        public ManualDownload(string title, string url)
        {
            Title = title;
            Url = url;
        }

        public string Title { get; set; }
        public string Url { get; set; }
    }
}
