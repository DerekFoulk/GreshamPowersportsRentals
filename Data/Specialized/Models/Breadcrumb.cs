using BlazorApp.Shared;

namespace Data.Specialized.Models
{
    public class Breadcrumb : Entity
    {
        public Breadcrumb(string text)
        {
            Text = text;
        }

        public string Text { get; set; }
    }
}
