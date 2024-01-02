using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Api
{
    public class RentalFunction
    {
        private readonly FakeDatastore _fakeDatastore;
        private readonly ILogger _logger;

        public RentalFunction(ILoggerFactory loggerFactory, FakeDatastore fakeDatastore)
        {
            _fakeDatastore = fakeDatastore;
            _logger = loggerFactory.CreateLogger<RentalFunction>();
        }

        [Function("Rentals")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
        {
            var result = _fakeDatastore.Rentals;

            var response = req.CreateResponse(HttpStatusCode.OK);

            await response.WriteAsJsonAsync(result);

            return response;
        }
    }
}
