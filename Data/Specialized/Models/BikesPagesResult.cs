namespace Data.Specialized.Models
{
    /// <summary>
    /// This class contains the results of scraping multiple bike list pages for bike details page urls
    /// </summary>
    public class BikesPagesResult
    {
        public BikesPagesResult(IEnumerable<BikesPageResult> bikesPageResults)
        {
            BikesPageResults = bikesPageResults;
        }

        public int? MinPage { get; set; }
        
        public int? MaxPage { get; set; }

        public IEnumerable<BikesPageResult> BikesPageResults { get; set; }
    }
}