using AutoMapper;
using BlazorApp.Shared.Models;
using Data.Husqvarna.Services;
using Data.Specialized.Services;

namespace Data.Services
{
    public class ModelsService : IModelsService
    {
        private readonly IMapper _mapper;
        private readonly IHusqvarnaBicyclesService _husqvarnaBicyclesService;
        private readonly ISpecializedBikesService _specializedBikesService;

        public ModelsService(IMapper mapper, IHusqvarnaBicyclesService husqvarnaBicyclesService, ISpecializedBikesService specializedBikesService)
        {
            _mapper = mapper;
            _husqvarnaBicyclesService = husqvarnaBicyclesService;
            _specializedBikesService = specializedBikesService;
        }

        public Task<IEnumerable<Model>> GetModelsAsync()
        {
            var models = new List<Model>();

            var husqvarnaBicycleInfos = _husqvarnaBicyclesService.GetBicycleInfos();

            foreach (var husqvarnaBicycleInfo in husqvarnaBicycleInfos)
            {
                var model = _mapper.Map<Model>(husqvarnaBicycleInfo);

                models.Add(model);
            }

            //var specializedModels = await _specializedBikesService.GetModelsAsync();

            //foreach (var specializedModel in specializedModels)
            //{
            //    var model = _mapper.Map<Model>(specializedModel);

            //    models.Add(model);
            //}

            return Task.FromResult<IEnumerable<Model>>(models);
        }
    }
}
