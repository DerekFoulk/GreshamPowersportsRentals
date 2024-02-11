using Data.Specialized.Models;

namespace Data.Specialized.Services
{
    public interface ISpecializedBikesService : IDisposable
    {
        SpecializedModel GetModel(string url);
        Task<IEnumerable<SpecializedModel>> GetModelsAsync(int? maxPage = null, int? minPage = null);
    }
}