using System.Net;
using System.Threading.Tasks;
using BlazorApp.Shared;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Api
{
    public class CategoryFunction
    {
        private readonly IDatastore _datastore;
        private readonly ILogger _logger;

        public CategoryFunction(ILoggerFactory loggerFactory, IDatastore datastore)
        {
            _datastore = datastore;
            _logger = loggerFactory.CreateLogger<CategoryFunction>();
        }

        [Function("Categories")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
        {
            var result = _datastore.Categories;
            
            var response = req.CreateResponse(HttpStatusCode.OK);

            await response.WriteAsJsonAsync(result);

            return response;
        }
    }
}
