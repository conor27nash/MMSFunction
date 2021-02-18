using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MMS.Function
{
    public class CosmosConnect
    {
        public static CosmosClient cosmosClient = new CosmosClient(Environment.GetEnvironmentVariable("COSMOS_CONNECTION", EnvironmentVariableTarget.Process));
        public static Database mms = cosmosClient.GetDatabase("mms");
        public static Container container = mms.GetContainer("MMSConfig");

        public static Config GetConfig(String inputID)
        {
           Config results = container.ReadItemAsync<Config>(inputID, new PartitionKey("")).Result;
            
            return results;
        }

        public static List<Config> GetAll()
        {
            QueryDefinition querySpec = new QueryDefinition($"SELECT * FROM m");
            FeedIterator<Config> resultsIterator = CosmosConnect.container.GetItemQueryIterator<Config>(querySpec);
            List<Config> results = new List<Config>();

            while (resultsIterator.HasMoreResults)
            {
                var task = resultsIterator.ReadNextAsync();
                task.Wait();
                results.AddRange(task.Result);

            }
            return results;
        }
        public async static void PostConfig(Config test)
        {
            ItemResponse<Config> item = await container.UpsertItemAsync<Config>(test, new PartitionKey(test.location));
        }

        public async static void deleteConfig(String id){
            ItemResponse<Config> configDelete = await container.DeleteItemAsync<Config>(id, new PartitionKey(""));
        }
    }

}
