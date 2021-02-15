using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;

namespace MMS.Function
{
    public class CosmosConnect
    {
        public static CosmosClient cosmosClient = new CosmosClient(Environment.GetEnvironmentVariable("COSMOS_CONNECTION", EnvironmentVariableTarget.Process));
        public static Database mms = cosmosClient.GetDatabase("mms");
        public static Container container = mms.GetContainer("MMSConfig");

        public static Config GetConfig(String inputID)
        {
            QueryDefinition querySpec = new QueryDefinition($"SELECT * FROM m WHERE m.id='{inputID}'");
            FeedIterator<Config> resultsIterator = CosmosConnect.container.GetItemQueryIterator<Config>(querySpec);
            List<Config> results = new List<Config>();

            while (resultsIterator.HasMoreResults)
            {
                var task = resultsIterator.ReadNextAsync();
                task.Wait();
                results.AddRange(task.Result);

            }

            if (results.Count < 1)
            {   
                Config notFound = new Config() 
                {id = "00:00:00:00:00",
                log_interval = 0,
                data_post_api = "unknown"
                 };
                return notFound;            
            }
           return results[0];

        }


        public async static void PostConfig(Config test){
            ItemResponse<Config> item = await container.UpsertItemAsync<Config>(test, new PartitionKey(test.location));
        }
    }

}
