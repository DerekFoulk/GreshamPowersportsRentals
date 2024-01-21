using Data.Specialized.Models;

namespace Data.Specialized.Services
{
    public interface ISpecializedBikesService
    {
        IEnumerable<Model> GetModels(int? maxBikes = null, int? minPage = null, int? maxPage = null);
        IEnumerable<Model> GetModels(int maxBikes, int minPage = 1);
        IEnumerable<Model> GetModelsFromPages(int maxPage);
        IEnumerable<Model> GetModelsFromPages(int maxPage, int minPage);
    }
}