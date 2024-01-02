using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Api
{
    public class ModelFunction
    {
        private readonly FakeDatastore _fakeDatastore;
        private readonly ILogger _logger;

        public ModelFunction(ILoggerFactory loggerFactory, FakeDatastore fakeDatastore)
        {
            _fakeDatastore = fakeDatastore;
            _logger = loggerFactory.CreateLogger<ModelFunction>();
        }

        [Function("Models")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
        {
            var result = _fakeDatastore.Models;

            var response = req.CreateResponse(HttpStatusCode.OK);

            await response.WriteAsJsonAsync(result);

            return response;
        }
    }
}
