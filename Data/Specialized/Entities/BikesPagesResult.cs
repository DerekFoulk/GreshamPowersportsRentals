using BlazorApp.Shared;

namespace Data.Specialized.Entities
{
    /// <summary>
    /// This class contains the results of scraping multiple bike list pages for bike details page urls
    /// </summary>
    public class BikesPagesResult : Entity
    {
        public BikesPagesResult() { }

        public int? MinPage { get; set; }

        public int? MaxPage { get; set; }

        public IEnumerable<BikesPageResult>? BikesPageResults { get; set; }
    }
}