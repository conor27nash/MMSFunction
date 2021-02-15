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

namespace MMS.Function
{
    public static class MMSConfigUpsert
    {
        [FunctionName("MMSConfigUpsert")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<Config>(requestBody);
            Config newConfig = new Config()
            {
                id = data.id,
                log_interval = data.log_interval,
                data_post_api = data.data_post_api,
                location = ""
            };
            Console.WriteLine($"input id: {newConfig.id}, input log: {newConfig.log_interval}, input data: {newConfig.data_post_api}");
            CosmosConnect.PostConfig(newConfig);
            return new OkObjectResult(newConfig);
        }
    }
}
