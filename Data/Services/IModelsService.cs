using BlazorApp.Shared.Models;

namespace Data.Services
{
    public interface IModelsService
    {
        Task<IEnumerable<Model>> GetModelsAsync();
    }
}