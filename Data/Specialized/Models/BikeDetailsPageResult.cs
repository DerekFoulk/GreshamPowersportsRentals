using Data.Specialized.Entities;

namespace Data.Specialized.Models
{
    /// <summary>
    /// This class contains the results of the scrape of a bike details page
    /// </summary>
    public class BikeDetailsPageResult
    {
        public BikeDetailsPageResult(string url, Model model)
        {
            Url = url;
            Model = model;
        }

        public string Url { get; set; }

        public Model Model { get; set; }
    }
}