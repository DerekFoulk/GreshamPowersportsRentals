using Data.Specialized.Models;

namespace Data.Specialized.Services
{
    public interface ISpecializedBikesService
    {
        IEnumerable<Model> GetModels();
    }
}