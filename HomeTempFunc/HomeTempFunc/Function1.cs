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
using System.Text;

namespace HomeTempFunc
{
    public static class Function1
    {
        static string connectionStr = Environment.GetEnvironmentVariable("sqldb_connection");

        [FunctionName("Temperature")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("hello world");

            //string name = req.Query["name"];

            // log.LogInformation(req.Method);

            try
            {
                if (req.Method == "POST")
                {
                    var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                    log.LogInformation(requestBody);
                    dynamic data = JsonConvert.DeserializeObject(requestBody);

                    // log.LogInformation(data);
                    if (data != null)
                    {
                        using (SqlConnection conn = new SqlConnection(connectionStr))
                        {
                            conn.Open();

                            // var text = "CREATE TABLE tempTable (Timestamp datetime, Temp int, humidity int, PRIMARY KEY (Timestamp));";
                            // var text = "DROP TABLE tempTable"; //  (Timestamp datetime, Temp int, humidity int)";

                            string text = String.Format("INSERT INTO tempTable (Timestamp, Temp, humidity) VALUES ('{0}', {1}, {2}); ", data?.Timestamp, data?.Temp, data?.humidity);

                            log.LogInformation(text);

                            using (SqlCommand cmd = new SqlCommand(text, conn))
                            {
                                // Execute the command and log the # rows affected.
                                var rows = await cmd.ExecuteNonQueryAsync();
                                log.LogInformation($"{rows} rows were updated");
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.LogInformation(e.Message);
            }

            return new OkObjectResult("hello world");
        }
    }
}
