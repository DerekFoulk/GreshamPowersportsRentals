using Data.Husqvarna.Models;

namespace Data.Husqvarna.Services
{
    public interface IHusqvarnaBicyclesService
    {
        List<HusqvarnaBicycleInfo> GetBicycleInfos();
    }
}