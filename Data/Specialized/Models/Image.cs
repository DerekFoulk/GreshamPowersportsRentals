using BlazorApp.Shared;

namespace Data.Specialized.Models
{
    public class Image : Entity
    {
        public Image(string url)
        {
            Url = url;
        }

        public string Url { get; set; }
    }
}
