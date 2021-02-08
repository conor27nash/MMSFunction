using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data.SqlClient;

namespace MMS.Function
{
    public static class MMS_Post
    {
        [FunctionName("MMS_Post")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<MMSData>(requestBody);
            string id = data.Id;
            float power = data.Power;
            float temp = data.Temperature;
            float rpm = data.Rpm;
            DateTime date = data.DateLogged;
            

            Console.WriteLine(data);

            Console.WriteLine(id + "  " + power + "  " + temp + "  " + rpm);

            string savedata = "INSERT into dbo.motor (LogDate, Id,Power,Rpm,Temperature) VALUES (@LogDate, @Id, @Power, @Rpm, @Temperature)";
            string connectionString = Environment.GetEnvironmentVariable("SQL_CONN", EnvironmentVariableTarget.Process);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                using (SqlCommand command = new SqlCommand(savedata))
                {
                    command.Connection = connection;
                    command.Parameters.AddWithValue("LogDate", date);
                    command.Parameters.AddWithValue("@Id", id);
                    command.Parameters.AddWithValue("@Power", power);
                    command.Parameters.AddWithValue("@Rpm", rpm);
                    command.Parameters.AddWithValue("@Temperature", temp);
                    try
                    {
                        connection.Open();
                        int recordsAffected = command.ExecuteNonQuery();
                    }
                    catch (SqlException e)
                    {
                        log.LogCritical(e.ToString());
                    }
                }


            }

            return new OkObjectResult(data);
        }
    }

}
