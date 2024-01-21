using BlazorApp.Shared;

namespace Data.Specialized.Entities
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
