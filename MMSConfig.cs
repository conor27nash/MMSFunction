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
            string deleteID = (req.Query["deleteID"]);
            

            if (deleteID != null)
            {
                CosmosConnect.deleteConfig(deleteID);
            }
            if (motorID != null)
            {
                if (motorID.ToLower() == "all")
                {
                    List<Config> allResult = CosmosConnect.GetAll();
                    return new OkObjectResult(allResult);
                }
                Config result = CosmosConnect.GetConfig(motorID);
                log.LogInformation($"\nInput Query: {motorID}");



                return new OkObjectResult(result);
            }
            return new OkObjectResult("Hello\nTo retrieve configurations use the query: '?motorID=' with either the motor id for the configuration you want to retrieve or with the word 'all' to get all.\nTo delete a configuration use the query: '?deleteID=' with the motor id you wish to remove.");
        }


    }
}

