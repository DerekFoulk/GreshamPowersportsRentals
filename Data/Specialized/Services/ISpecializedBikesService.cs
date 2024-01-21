using Data.Specialized.Models;

namespace Data.Specialized.Services
{
    public interface ISpecializedBikesService
    {
        BikesResult GetBikes(int? maxBikes = null, int? minPage = null, int? maxPage = null);
        BikesResult GetBikes(int maxBikes, int minPage = 1);
        BikesResult GetBikesFromPages(int maxPage);
        BikesResult GetBikesFromPages(int maxPage, int minPage);
    }
}