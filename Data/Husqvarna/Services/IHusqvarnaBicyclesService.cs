using Data.Husqvarna.Models;

namespace Data.Husqvarna.Services
{
    public interface IHusqvarnaBicyclesService : IDisposable
    {
        HusqvarnaBicycleInfo GetBicycleInfo(string url);
        IEnumerable<HusqvarnaBicycleInfo> GetBicycleInfos();
    }
}