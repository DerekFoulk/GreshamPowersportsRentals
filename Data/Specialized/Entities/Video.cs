using BlazorApp.Shared;

namespace Data.Specialized.Entities
{
    public class Video : Entity
    {
        public Video(string url)
        {
            Url = url;
        }

        public string Url { get; set; }
    }
}
