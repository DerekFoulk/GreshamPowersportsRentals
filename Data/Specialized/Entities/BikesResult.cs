using BlazorApp.Shared;

namespace Data.Specialized.Entities
{
    /// <summary>
    /// This class contains the results of the entire scrape of Specialized's website
    /// </summary>
    public class BikesResult : Entity
    {
        public BikesResult(BikesPagesResult bikesPagesResult, IEnumerable<BikeDetailsPageResult> bikeDetailsPageResults)
        {
            BikesPagesResult = bikesPagesResult;
            BikeDetailsPageResults = bikeDetailsPageResults;
        }

        public BikesPagesResult BikesPagesResult { get; set; }

        public IEnumerable<BikeDetailsPageResult> BikeDetailsPageResults { get; set; }

        public int? MaxBikes { get; set; }

        public int? MinPage { get; set; }

        public int? MaxPage { get; set; }
    }
}