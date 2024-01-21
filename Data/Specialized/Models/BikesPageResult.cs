namespace Data.Specialized.Models
{
    public class BikesPageResult
    {
        public BikesPageResult(string url, int pageNumber, IEnumerable<string> urls)
        {
            Url = url;
            PageNumber = pageNumber;
            Urls = urls;
        }

        public string Url { get; set; }

        public int PageNumber { get; set; }

        public IEnumerable<string> Urls { get; set; }
    }
}
