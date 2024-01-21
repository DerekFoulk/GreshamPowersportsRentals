namespace Data.Specialized.Models
{
    /// <summary>
    /// This class contains the results of the entire scrape of Specialized's website
    /// </summary>
    public class BikesResult
    {
        public BikesResult(BikesPagesResult bikesPagesResult, IEnumerable<BikeDetailsPageResult> bikeDetailsPageResults)
        {
            BikesPagesResult = bikesPagesResult;
            BikeDetailsPageResults = bikeDetailsPageResults;
        }

        public BikesPagesResult BikesPagesResult { get; set; }

        public IEnumerable<BikeDetailsPageResult> BikeDetailsPageResults { get; set; }

        public IEnumerable<Exception>? Exceptions { get; set; }
    }
}