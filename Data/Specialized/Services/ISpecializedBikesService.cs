using Data.Specialized.Models;

namespace Data.Specialized.Services
{
    public interface ISpecializedBikesService
    {
        Task<IEnumerable<Model>> GetModelsAsync();
    }
}