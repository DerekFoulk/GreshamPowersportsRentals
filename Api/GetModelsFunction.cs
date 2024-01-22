using System;
using System.Net;
using System.Threading.Tasks;
using BlazorApp.Shared.Contexts;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Api
{
    public class GetModelsFunction
    {
        private readonly IDbContextFactory<RentalsContext> _dbContextFactory;
        private readonly ILogger _logger;

        public GetModelsFunction(ILoggerFactory loggerFactory, IDbContextFactory<RentalsContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
            _logger = loggerFactory.CreateLogger<GetModelsFunction>();
        }

        [Function("GetModelsFunction")]
        public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            _logger.LogTrace("C# HTTP trigger function has received a request");

            try
            {
                var response = req.CreateResponse(HttpStatusCode.OK);
                response.Headers.Add("Content-Type", "application/json; charset=utf-8");

                _logger.LogTrace("Getting models from Azure Cosmos DB");

                await using var context = await _dbContextFactory.CreateDbContextAsync();
                var models = context.Models;

                await response.WriteAsJsonAsync(models);

                _logger.LogTrace($"Returning {await models.CountAsync()} models as JSON from Azure Cosmos DB ({response.StatusCode})");

                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to get models from Azure Cosmos DB");

                throw;
            }
        }
    }
}
