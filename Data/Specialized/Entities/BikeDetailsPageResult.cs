using BlazorApp.Shared;
using Model = Data.Specialized.Entities.Model;

namespace Data.Specialized.Entities
{
    /// <summary>
    /// This class contains the results of the scrape of a bike details page
    /// </summary>
    public class BikeDetailsPageResult : Entity
    {
        public BikeDetailsPageResult() { }

        public BikeDetailsPageResult(string url, Model model)
        {
            Url = url;
            Model = model;
        }

        public string Url { get; set; }

        public Model Model { get; set; }
    }
}