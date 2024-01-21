using Data.Specialized.Entities;

namespace Data.Specialized.Services
{
    public interface ISpecializedBikesService
    {
        Task<BikesResult> GetBikesAsync(int? maxBikes = null, int? minPage = null, int? maxPage = null);
        Task<BikesResult> GetBikesAsync(int maxBikes, int minPage = 1);
        Task<BikesResult> GetBikesFromPagesAsync(int maxPage);
        Task<BikesResult> GetBikesFromPagesAsync(int maxPage, int minPage);
    }
}