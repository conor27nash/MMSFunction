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
using System.Collections.Generic;

namespace MMS.Function
{
    public static class MMSRetrieveData
    {
        [FunctionName("MMSRetrieveData")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            List<List<MMSGraphPoint>> MotorList = new List<List<MMSGraphPoint>>();
            List<MMSGraphPoint> powerList = new List<MMSGraphPoint>();
            List<MMSGraphPoint> rpmList = new List<MMSGraphPoint>();
            List<MMSGraphPoint> tempList = new List<MMSGraphPoint>();
            MotorList.Add(powerList);
            MotorList.Add(rpmList);
            MotorList.Add(tempList);
            string motorID = req.Query["motorID"];

            string getQuery = $"SELECT * FROM dbo.motor WHERE id='{motorID}'";
            string connectionString = Environment.GetEnvironmentVariable("SQL_CONN", EnvironmentVariableTarget.Process);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(getQuery, connection);
                connection.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        // Console.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}", dr.GetDateTime(0),
                        // dr.GetString(1), dr.GetFloat(2), dr.GetFloat(3), dr.GetFloat(4));
                        MMSGraphPoint powerPoint = new MMSGraphPoint()
                        {
                            t = dr.GetDateTime(0),
                            y = dr.GetFloat(2)
                        };
                        powerList.Add(powerPoint);
                        MMSGraphPoint rpmPoint = new MMSGraphPoint()
                        {
                            t = dr.GetDateTime(0),
                            y = dr.GetFloat(3)
                        };
                        rpmList.Add(rpmPoint);
                        MMSGraphPoint tempPoint = new MMSGraphPoint()
                        {
                            t = dr.GetDateTime(0),
                            y = dr.GetFloat(4)
                        };
                        tempList.Add(tempPoint);
                    }
                }
                else
                {
                    Console.WriteLine("No rows found.");
                }
                Console.WriteLine(dr.ToString());
                dr.Close();

            }
            Console.WriteLine(MotorList);

            return new OkObjectResult(MotorList);
        }
    }
}
