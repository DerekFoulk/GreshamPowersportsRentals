using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Api
{
    public class ModelsFunction
    {
        private readonly IDatastore _datastore;
        private readonly ILogger _logger;

        public ModelsFunction(ILoggerFactory loggerFactory, IDatastore datastore)
        {
            _datastore = datastore;
            _logger = loggerFactory.CreateLogger<ModelsFunction>();
        }

        [Function("Models")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
        {
            var result = _datastore.Models;

            var response = req.CreateResponse(HttpStatusCode.OK);

            try
            {
                await response.WriteAsJsonAsync(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to write response as JSON");
                
                throw;
            }

            return response;
        }
    }
}
