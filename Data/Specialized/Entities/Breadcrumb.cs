using BlazorApp.Shared;

namespace Data.Specialized.Entities
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
