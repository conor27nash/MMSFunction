using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos;
using System.Collections.Generic;

namespace MMS.Function
{
    public static class MMSConfig
    {
        [FunctionName("MMSConfig")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            CosmosClient cosmosClient = new CosmosClient(
            Environment.GetEnvironmentVariable("COSMOS_CONNECTION",EnvironmentVariableTarget.Process));
            Database mms = cosmosClient.GetDatabase("mms");
            Container container = mms.GetContainer("MMSConfig");

            log.LogInformation($"Client making request:  {req.Headers["client-ip"]}");


           string motorID = (req.Query["motorID"]);
           log.LogInformation($"\nInput Query: {motorID}");

           QueryDefinition querySpec = new QueryDefinition($"SELECT * FROM m WHERE m.id='{motorID}'");
           FeedIterator<Config> resultsIterator = container.GetItemQueryIterator<Config>(querySpec);
           List<Config> results = new List<Config>();
           while (resultsIterator.HasMoreResults)
            {
                var task = resultsIterator.ReadNextAsync();
                task.Wait();
                results.AddRange(task.Result);

            }
            log.LogInformation($"found {results}");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
           
            return new OkObjectResult(results[0]);
        }
    }
}
