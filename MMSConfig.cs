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
            

            log.LogInformation($"Client making request:  {req.Headers["client-ip"]}");

            
            string motorID = (req.Query["motorID"]);
            Config result = CosmosConnect.GetConfig(motorID);
            log.LogInformation($"\nInput Query: {motorID}");

           

            return new OkObjectResult(result);
        }
        

    }
}

